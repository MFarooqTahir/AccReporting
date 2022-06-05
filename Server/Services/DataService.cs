using AccountsReportsWASM.Shared.ReportModels;

using AccReporting.Server.DbContexts;
using AccReporting.Shared;
using AccReporting.Shared.DTOs;

using EFCore.BulkExtensions;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Caching.Memory;

using System.Data;
using System.Diagnostics;

namespace AccReporting.Server.Services;

public class DataService
{
    private readonly IMemoryCache? _cache;
    private readonly AccountInfoDbContext _Db;
    private readonly IConfiguration _config;
    private string _ConnectionString = "";

    private string ConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_ConnectionString))
            {
                _ConnectionString = _config.GetConnectionString("NoDb") + "Database=" + _DbName + ";";
            }
            return _ConnectionString;
        }

        set
        {
            _ConnectionString = value;
        }
    }

    private string _DbName;
    public string CachePrefix => _DbName;

    public DataService(IMemoryCache cache, AccountInfoDbContext db, IConfiguration config)
    {
        _cache = cache;
        _Db = db;
        _config = config;
    }

    public async Task SetDbName(string dbName, CancellationToken ct)
    {
        if (string.Equals(dbName, _DbName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        _DbName = dbName;
        ConnectionString = "";
        await EnsureDbConenction(ct);
    }

    public async Task<IEnumerable<InvSummGridModel?>?> GetInvSummGridAsync(string AcCode, int PageNumber, int PageSize, CancellationToken ct)
    {
        string key = $"SRG-{AcCode}";
        var suc = GetCache(key, out IEnumerable<InvSummGridModel> ret);
        if (!suc)
        {
            ret = (await GetInvDetAsync(ct))
                .Where(x => x.Pcode == AcCode)
             .GroupBy(x => new { x.InvNo, x.Sp })
             .Select(x => new InvSummGridModel(x.Key.InvNo, x.Key.Sp, x.Sum(z => z.Amount)));

            SetCache(key, ret);
        }
        if (PageSize > 0 || PageNumber > 0)
        {
            ret = ret.Skip(PageNumber * PageSize).Take(PageSize);
        }
        return ret;
    }

    public async Task<SalesReportDto?> GetSalesInvoiceData(int invNo, string Type, string AcNumber, CancellationToken ct)
    {
        string reportKey = "SR-" + invNo + "-" + Type + "-" + AcNumber;
        var successful = GetCache(reportKey, out SalesReportDto? ret);
        if (!successful)
        {
            ret = new SalesReportDto()
            {
                CompanyName = "ABC Company",
                Type = Type,
            };
            //_Db.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));

            var dataSumm = (await GetInvSummsAsync(ct).ConfigureAwait(false))
                          .Where(x => x.InvNo == invNo && x.Pcode == AcNumber)
                         .Select(x => new { x.Payment, x.RefNo, x.DueDate, x.InvDate, x.InvNo })
                         .FirstOrDefault();
            if (dataSumm is not null)
            {
                ret.RefNumber = dataSumm.RefNo;
                ret.Dated = dataSumm.InvDate;
                ret.DueDate = dataSumm.DueDate;
                ret.Payment = dataSumm.Payment;
                ret.InvNo = dataSumm.InvNo ?? 0;
            }
            if (ret.InvNo == 0)
            {
                return ret;
            }
            ret.tableData ??= new();
            ret.tableData.AddRange(
                            (await GetInvDetAsync(ct).ConfigureAwait(false))
                            .Where(x => x.Pcode == AcNumber && x.InvNo == invNo && x.Sp == Type)
                            .Select(
                                    row => new SalesReportModel()
                                    {
                                        Amount = row.Amount,
                                        NetAmount = row.NetAmount,
                                        Rate = row.Rate,
                                        Quantity = (int)(row.Qty ?? 0.0),
                                        unit = row.Unit,
                                        Description = row.Iname,
                                        Discount = row.Dper
                                    }
                            )
                            );
            SetCache(reportKey, ret);
        }
        return ret;
    }

    public async Task<bool> InsertAllDataBulk(string access, CancellationToken ct)
    {
        try
        {
            List<Acfile> AcFileInsert = new();
            List<InvDet> InvDetInsert = new();
            List<Inventory> InventoryInsert = new();
            List<InvSumm> InvSummInsert = new();
            List<Trans> TransInsert = new();
            string[]? splitData = access.Split("---START---", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Parallel.ForEach(splitData, SplitEntries =>
            {
                string[]? entries = SplitEntries.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);//.Where(x => !(x.Contains("DROP", StringComparison.InvariantCultureIgnoreCase) && x.Contains("DELETE FROM", StringComparison.InvariantCultureIgnoreCase) && x.Contains("UPDATE", StringComparison.InvariantCultureIgnoreCase)));
                IEnumerable<string[]>? newEnt = entries.Skip(1).Select(x => x.Split("|"));
                switch (entries[0].ToUpperInvariant())
                {
                    case "ACFILE":
                        AcFileInsert.AddRange(newEnt.Select(x => new Acfile(x)));
                        break;

                    case "INVDET":
                        InvDetInsert.AddRange(newEnt.Where(x => !string.IsNullOrWhiteSpace(x[0])).Select(x => new InvDet(x)));
                        break;

                    case "INVENTORY":
                        InventoryInsert.AddRange(newEnt.Select(x => new Inventory(x)));
                        break;

                    case "INVSUMM":
                        InvSummInsert.AddRange(newEnt.Where(x => !string.IsNullOrWhiteSpace(x[1])).Select(x => new InvSumm(x)));
                        break;

                    case "TRANS":
                        TransInsert.AddRange(newEnt.Select(x => new Trans(x)));
                        break;
                }
            });
            try
            {
                await EnsureDbConenction(ct);
                await _Db.Database.EnsureCreatedAsync(ct);
                using (var trans = await _Db.Database.BeginTransactionAsync(ct).ConfigureAwait(false))
                {
                    try
                    {
                        await _Db.BulkInsertAsync(AcFileInsert, cancellationToken: ct).ConfigureAwait(false);
                        await _Db.BulkInsertAsync(InvDetInsert, cancellationToken: ct).ConfigureAwait(false);
                        await _Db.BulkInsertAsync(InventoryInsert, cancellationToken: ct).ConfigureAwait(false);
                        await _Db.BulkInsertAsync(InvSummInsert, cancellationToken: ct).ConfigureAwait(false);
                        await _Db.BulkInsertAsync(TransInsert, cancellationToken: ct).ConfigureAwait(false);
                        await trans.CommitAsync(ct).ConfigureAwait(false);
                        ClearDbCache();
                        SetAcFile(AcFileInsert);
                        SetInventory(InventoryInsert);
                        SetTrans(TransInsert);
                        SetInvSumm(InvSummInsert);
                        SetInvDet(InvDetInsert);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
#endif
                        await trans.RollbackAsync(ct).ConfigureAwait(false);
                        return false;
                    }
                }
                await _Db.Database.CloseConnectionAsync();
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
#endif
                return false;
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
#endif
            return false;
        }
    }

    private void SetInvSumm(IEnumerable<InvSumm> Data) => _cache.Set(CachePrefix + "-InvSumm", Data);

    private async Task<IEnumerable<InvSumm>> GetInvSummsAsync(CancellationToken ct)
    {
        var res = _cache.TryGetValue(CachePrefix + "-InvSumm", out IEnumerable<InvSumm> Data);
        if (!res)
        {
            await EnsureDbConenction(ct);
            Data = await _Db.InvSumms.AsNoTracking().ToListAsync(ct).ConfigureAwait(false);
            SetInvSumm(Data);
        }
        return Data;
    }

    private void SetAcFile(IEnumerable<Acfile> Data) => _cache.Set(CachePrefix + "-AcFile", Data);

    private async Task<IEnumerable<Acfile>> GetAcFileAsync(CancellationToken ct)
    {
        var res = _cache.TryGetValue(CachePrefix + "-AcFile", out IEnumerable<Acfile> Data);
        if (!res)
        {
            await EnsureDbConenction(ct);
            Data = await _Db.Acfiles.AsNoTracking().ToListAsync(ct).ConfigureAwait(false);
            SetAcFile(Data);
        }
        return Data;
    }

    private void SetInventory(IEnumerable<Inventory> Data) => _cache.Set(CachePrefix + "-Inventory", Data);

    private async Task<IEnumerable<Inventory>> GetInventory(CancellationToken ct)
    {
        var res = _cache.TryGetValue(CachePrefix + "-Inventory", out IEnumerable<Inventory> Data);
        if (!res)
        {
            await EnsureDbConenction(ct);
            Data = await _Db.Inventories.AsNoTracking().ToListAsync(ct).ConfigureAwait(false);
            SetInventory(Data);
        }
        return Data;
    }

    private void SetInvDet(IEnumerable<InvDet> Data) => _cache.Set(CachePrefix + "-InvDet", Data);

    private async Task<IEnumerable<InvDet>> GetInvDetAsync(CancellationToken ct)
    {
        var res = _cache.TryGetValue(CachePrefix + "-InvDet", out IEnumerable<InvDet> Data);
        if (!res)
        {
            await EnsureDbConenction(ct);
            Data = await _Db.InvDets.AsNoTracking().ToListAsync(ct).ConfigureAwait(false);
            SetInvDet(Data);
        }
        return Data;
    }

    private void SetTrans(IEnumerable<Trans> Data) => _cache.Set(CachePrefix + "-Trans", Data);

    private async Task<IEnumerable<Trans>> GetTransAsync(CancellationToken ct)
    {
        var res = _cache.TryGetValue(CachePrefix + "-Trans", out IEnumerable<Trans> Data);
        if (!res)
        {
            await EnsureDbConenction(ct);
            Data = await _Db.Trans.AsNoTracking().ToListAsync(ct).ConfigureAwait(false);
            SetTrans(Data);
        }
        return Data;
    }

    private async Task EnsureDbConenction(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_DbName))
        {
            throw new DbEmptyException("No database name set");
        }
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new DbEmptyException("No connection string set");
        }
        string db = _Db.Database.GetDbConnection().Database;
        if (db != _DbName || !await _Db.Database.CanConnectAsync(ct))
        {
            _Db.Database.SetConnectionString(ConnectionString);
        }
    }

    private void SetCache<T>(string key, T value)
    {
        if (_cache is not null)
        {
            _cache.Set(CachePrefix + key, value);
        }
    }

    private bool GetCache<T>(string key, out T Result)
    {
        Result = default!;
        if (_cache is not null)
        {
            return _cache.TryGetValue(CachePrefix + key, out Result);
        }
        return false;
    }

    private void ClearDbCache()
    {
        var keysList = _cache.GetKeysForDb(CachePrefix);
        if (keysList is not null && keysList.Any())
        {
            foreach (var key in keysList)
                _cache.Remove(key);
        }
    }
}