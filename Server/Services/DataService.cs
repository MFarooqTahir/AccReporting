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
    public string AccountNumber { get; set; }
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

    public async Task SetDbName(string dbName)
    {
        if (string.Equals(dbName, _DbName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        _DbName = dbName;
        ConnectionString = "";
        await EnsureDbConenction();
    }

    private async Task EnsureDbConenction()
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
        if (db != _DbName || !await _Db.Database.CanConnectAsync())
        {
            _Db.Database.SetConnectionString(ConnectionString);
        }
    }

    private bool SetCache<T>(string key, T value)
    {
        if (_cache is not null)
        {
            _cache.Set(CachePrefix + key, value);
            return true;
        }
        return false;
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

    public async Task<SalesReportDto> GetSalesInvoiceData(int invNo, string Type, CancellationToken ct)
    {
        var ret = new SalesReportDto()
        {
            CompanyName = "ABC Company",
        };
        _Db.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));
        var dbR = _Db.InvDets.AsNoTracking()
                        .Where(x => x.InvNo == invNo && x.Sp == Type);
        var Pcode = await dbR.Select(x => x.Pcode).FirstOrDefaultAsync(ct);

        var dataSumm = await _Db.InvSumms.AsNoTracking()
                      .Where(x => x.InvNo == invNo && x.Pcode == Pcode)
                     .Select(x => new { x.Pname, x.RefNo, x.DueDate, x.InvDate, x.InvNo })
                     .FirstOrDefaultAsync(ct);
        if (dataSumm is not null)
        {
            ret.RefNumber = dataSumm.RefNo;
            ret.Dated = dataSumm.InvDate;
            ret.DueDate = dataSumm.DueDate;
            ret.NameAndAddress = dataSumm.Pname;
            ret.InvNo = dataSumm.InvNo ?? 0;
        }
        var tableRes =
            dbR.Select(x => new { x.Amount, x.Rate, x.NetAmount, x.Dper, x.Iname, Qty = (int)(x.Qty ?? 0), x.Unit })
            .AsEnumerable();
        ret.tableData = new();
        ret.tableData.AddRange(tableRes.Select(
            row => new SalesReportModel()
            {
                Amount = row.Amount,
                NetAmount = row.NetAmount,
                Rate = row.Rate,
                Quantity = row.Qty,
                unit = row.Unit,
                Description = row.Iname,
                Discount = row.Dper
            }));
        return ret;
    }

    public async Task<bool> InsertAllDataBulk(string access)
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
                //foreach (var SplitEntries in splitData)
                //{
                string[]? entries = SplitEntries.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);//.Where(x => !(x.Contains("DROP", StringComparison.InvariantCultureIgnoreCase) && x.Contains("DELETE FROM", StringComparison.InvariantCultureIgnoreCase) && x.Contains("UPDATE", StringComparison.InvariantCultureIgnoreCase)));
                IEnumerable<string[]>? newEnt = entries.Skip(1).Select(x => x.Split("|"));
                switch (entries[0].ToUpperInvariant())
                {
                    case "ACFILE":
                        AcFileInsert.AddRange(newEnt.Select(x => new Acfile(x)));
                        break;

                    case "INVDET":
                        InvDetInsert.AddRange(newEnt.Select(x => new InvDet(x)));
                        break;

                    case "INVENTORY":
                        InventoryInsert.AddRange(newEnt.Select(x => new Inventory(x)));
                        break;

                    case "INVSUMM":
                        InvSummInsert.AddRange(newEnt.Select(x => new InvSumm(x)));
                        break;

                    case "TRANS":
                        TransInsert.AddRange(newEnt.Select(x => new Trans(x)));
                        break;
                }
                //}
            });
            try
            {
                await EnsureDbConenction();
                await _Db.Database.EnsureCreatedAsync();
                using (var trans = await _Db.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        await _Db.BulkInsertAsync(AcFileInsert).ConfigureAwait(false);
                        await _Db.BulkInsertAsync(InvDetInsert).ConfigureAwait(false);
                        await _Db.BulkInsertAsync(InventoryInsert).ConfigureAwait(false);
                        await _Db.BulkInsertAsync(InvSummInsert).ConfigureAwait(false);
                        await _Db.BulkInsertAsync(TransInsert).ConfigureAwait(false);
                        await trans.CommitAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
#endif
                        await trans.RollbackAsync().ConfigureAwait(false);
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

    //    public async Task<bool> InsertAllDataBulkFor(string access)
    //    {
    //        try
    //        {
    //            List<Acfile> AcFileInsert = new();
    //            List<InvDet> InvDetInsert = new();
    //            List<Inventory> InventoryInsert = new();
    //            List<InvSumm> InvSummInsert = new();
    //            List<Trans> TransInsert = new();
    //            //Parallel.ForEach(access.Split("---START---", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), SplitEntries =>
    //            //{
    //            foreach (string? SplitEntries in access.Split("---START---", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    //            {
    //                string[]? entries = SplitEntries.Split(new string[] { "\r\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);//.Where(x => !(x.Contains("DROP", StringComparison.InvariantCultureIgnoreCase) && x.Contains("DELETE FROM", StringComparison.InvariantCultureIgnoreCase) && x.Contains("UPDATE", StringComparison.InvariantCultureIgnoreCase)));
    //                string? first = entries[0];
    //                entries = entries[1..];
    //                List<string[]> newEnt = new();
    //                for (int i = 0; i < entries.Length; i++)
    //                {
    //                    newEnt.Add(entries[i].Split("|"));
    //                }
    //                switch (first)
    //                {
    //                    case "ACFILE":
    //                        for (int i = 0; i < newEnt.Count; i++)
    //                        {
    //                            AcFileInsert.Add(new(newEnt[i]));
    //                        }
    //                        break;

    //                    case "INVDET":
    //                        for (int i = 0; i < newEnt.Count; i++)
    //                        {
    //                            InvDetInsert.Add(new(newEnt[i]));
    //                        }
    //                        break;

    //                    case "INVENTORY":
    //                        for (int i = 0; i < newEnt.Count; i++)
    //                        {
    //                            InventoryInsert.Add(new(newEnt[i]));
    //                        }
    //                        break;

    //                    case "INVSUMM":
    //                        for (int i = 0; i < newEnt.Count; i++)
    //                        {
    //                            InvSummInsert.Add(new(newEnt[i]));
    //                        }
    //                        break;

    //                    case "TRANS":
    //                        for (int i = 0; i < newEnt.Count; i++)
    //                        {
    //                            TransInsert.Add(new(newEnt[i]));
    //                        }
    //                        break;
    //                }
    //            }
    //            //});
    //            try
    //            {
    //                await EnsureDbConenction();

    //                await _Db.Database.EnsureDeletedAsync().ConfigureAwait(false);
    //                await _Db.Database.MigrateAsync().ConfigureAwait(false);
    //                using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? trans = await _Db.Database.BeginTransactionAsync().ConfigureAwait(false))
    //                {
    //                    try
    //                    {
    //                        await _Db.BulkInsertAsync(AcFileInsert).ConfigureAwait(false);
    //                        await _Db.BulkInsertAsync(InvDetInsert).ConfigureAwait(false);
    //                        await _Db.BulkInsertAsync(InventoryInsert).ConfigureAwait(false);
    //                        await _Db.BulkInsertAsync(InvSummInsert).ConfigureAwait(false);
    //                        await _Db.BulkInsertAsync(TransInsert).ConfigureAwait(false);
    //                        //await DbContext.BulkInsertAsync(InventoryList).ConfigureAwait(false);
    //                        //await DbContext.BulkInsertAsync(InvSummList).ConfigureAwait(false);
    //                        await trans.CommitAsync().ConfigureAwait(false);
    //                    }
    //                    catch (Exception ex)
    //                    {
    //#if DEBUG
    //                        Debug.WriteLine(ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
    //#endif
    //                        await trans.RollbackAsync().ConfigureAwait(false);
    //                        return false;
    //                    }
    //                }
    //                return true;
    //            }
    //            catch (Exception ex)
    //            {
    //#if DEBUG
    //                Debug.WriteLine(ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
    //#endif
    //                return false;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //#if DEBUG
    //            Debug.WriteLine(ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
    //#endif
    //            return false;
    //        }
    //    }
}