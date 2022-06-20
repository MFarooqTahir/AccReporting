using AccountsReportsWASM.Shared.ReportModels;

using AccReporting.Server.DbContexts;
using AccReporting.Shared;
using AccReporting.Shared.DTOs;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Caching.Memory;

using System.Data;
using System.Diagnostics;

namespace AccReporting.Server.Services;

public class DataService
{
    private readonly IMemoryCache? _cache;
    private readonly AccountInfoDbContext _Db;
    private readonly ILogger<DataService> _logger;
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

    public DataService(IMemoryCache cache, AccountInfoDbContext db, IConfiguration config, ILogger<DataService> logger)
    {
        _cache = cache;
        _Db = db;
        _config = config;
        _logger = logger;
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
            var R = await GetInvDetAsync(ct);
            if (!string.IsNullOrWhiteSpace(AcCode))
            {
                R = R.Where(x => x.Pcode == AcCode);
            }
            var contList = new[] { "S", "E", "P", "R" };
            ret = R.Where(x => contList.Contains(x.Sp))
                .OrderBy(x => x.InvNo)
             .GroupBy(x => new { x.InvNo, x.Sp })
             .Select(x => new InvSummGridModel(x.Key.InvNo, x.Key.Sp, x.Sum(z => z.Amount), x.Sum(z => z.NetAmount)));

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
                Type = Type,
            };
            //_Db.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));

            var dataSummx = (await GetInvSummsAsync(ct).ConfigureAwait(false))
                          .Where(x => x.InvNo == invNo);
            if (!string.IsNullOrWhiteSpace(AcNumber))
            {
                dataSummx = dataSummx.Where(x => x.Pcode == AcNumber);
            }
            var dataSumm = dataSummx.Select(x => new { x.Payment, x.RefNo, x.DueDate, x.InvDate, x.InvNo })
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
            var R = await GetInvDetAsync(ct).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(AcNumber))
            {
                R = R.Where(x => x.Pcode == AcNumber);
            }
            ret.tableData.AddRange(
                            R.Where(x => x.InvNo == invNo && x.Sp == Type)
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
                _logger.LogInformation("Starting Insert");
                await _Db.Database.EnsureDeletedAsync(ct);
                await _Db.Database.MigrateAsync(ct);
                _logger.LogInformation("Migration Done");
                using (var trans = await _Db.Database.BeginTransactionAsync(ct).ConfigureAwait(false))
                {
                    string query = "";
                    try
                    {
                        foreach (var acfile in AcFileInsert.Select(x => x.ToInsert()).Chunk(7000))
                        {
                            query = "INSERT INTO `acfile`(`ActCode`,`ActName`,`Address1`,`Address2`,`Address3`,`CrDays`,`email`,`fax`,`GST`,`OpBal`,`phone`) VALUES " +
                                string.Join(',', acfile) + ";";
                            await _Db.Database.ExecuteSqlRawAsync(query, ct);
                        }

                        foreach (var invdet in InvDetInsert.Select(x => x.ToInsert()).Chunk(7000))
                        {
                            query = "INSERT INTO `invdet`(`InvNo`,`InvDate`,`PCode`,`ICode`,`IName`,`Qty`,`Qty2`,`Unit`,`Packing`,`Rate`,`Amount`,`NetAmount`,`SP`,`Type`,`pName`,`Size`,`Pressure`,`CateCode`,`Dper`,`RegionCode`,`RegionName`,`FILE`) VALUES " +
                                string.Join(',', invdet) + ";";
                            await _Db.Database.ExecuteSqlRawAsync(query, ct);
                        }

                        foreach (var InvSumm in InvSummInsert.Select(x => x.ToInsert()).Chunk(7000))
                        {
                            query = "INSERT INTO `invsumm`(`OrderNo`,`InvNo`,`InvDate`,`PCode`,`PName`, `TotBill`,`Built`,`DisPer`,`Dis`,`Ser`,`Remarks`,`cartage`,`AddLess`,`CrDays`,`DueDate`,`RefNo`,`Payment`,`Note`,`Delivery`,`HCode`) VALUES " +
                                string.Join(',', InvSumm) + ";";
                            await _Db.Database.ExecuteSqlRawAsync(query, ct);
                        }

                        foreach (var Inventory in InventoryInsert.Select(x => x.ToInsert()).Chunk(7000))
                        {
                            query = "INSERT INTO `inventory`(`ItemCode`,`ItemDescrip`,`MfcCode`,`ManuName`,`Size`,`Pressure`,`Length`,`Price`,`RetPrice`,`RetPrice2`,`Unit`,`OpBal`) VALUES " +
                                string.Join(',', Inventory) + ";";
                            await _Db.Database.ExecuteSqlRawAsync(query, ct);
                        }

                        foreach (var Trans in TransInsert.Select(x => x.ToInsert()).Chunk(7000))
                        {
                            query = "INSERT INTO `trans`(`ActCode`,`ActName`,`ChqDate`,`ChqNo`,`Date`,`Des`,`TransAmt`,`Vnoc`,`Vnon`) VALUES " +
                                string.Join(',', Trans) + ";";
                            await _Db.Database.ExecuteSqlRawAsync(query, ct);
                        }
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

    public async Task<IEnumerable<InvSumm>> GetInvSummsAsync(CancellationToken ct)
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

    public async Task<IEnumerable<Acfile>> GetAcFileAsync(CancellationToken ct)
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

    public async Task<IEnumerable<Inventory>> GetInventory(CancellationToken ct)
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

    public async Task<IEnumerable<InvDet>> GetInvDetAsync(CancellationToken ct)
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

    public async Task<IEnumerable<Trans>> GetTransAsync(CancellationToken ct)
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