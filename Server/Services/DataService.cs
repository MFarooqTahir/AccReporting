using AccountsReportsWASM.Shared.ReportModels;

using AccReporting.Server.DbContexts;
using AccReporting.Shared;
using AccReporting.Shared.DTOs;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Caching.Memory;

using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;

namespace AccReporting.Server.Services;

public class DataService
{
    private readonly IMemoryCache? _cache;
    private readonly AccountInfoDbContext _db;
    private readonly ILogger<DataService> _logger;
    private readonly IConfiguration _config;
    private string _connectionString = "";

    private string ConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(value: _connectionString))
            {
                _connectionString = _config.GetConnectionString(name: "NoDb") + "Database=" + _dbName + ";";
            }
            return _connectionString;
        }

        set
        {
            _connectionString = value;
        }
    }

    private string _dbName;
    public string CachePrefix => _dbName;

    public DataService(IMemoryCache cache, AccountInfoDbContext db, IConfiguration config, ILogger<DataService> logger)
    {
        _cache = cache;
        _db = db;
        _config = config;
        _logger = logger;
    }

    public async Task SetDbName(string dbName, CancellationToken ct)
    {
        if (string.Equals(a: dbName, b: _dbName, comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        _dbName = dbName;
        ConnectionString = "";
        await EnsureDbConnection(ct: ct);
    }

    public async Task<IEnumerable<InvSummGridModel?>?> GetInvSummGridAsync(string acCode, string type, int pageNumber, int pageSize, CancellationToken ct)
    {
        var key = $"SRG-{acCode}-{type}-{pageNumber}-{pageSize}";
        var suc = GetCache(key: key, result: out IEnumerable<InvSummGridModel> ret);
        if (!suc)
        {
            await EnsureDbConnection(ct: ct);
            var a = _db.InvDets.AsNoTracking().Where(predicate: x => x.Sp == type);
            if (!string.IsNullOrEmpty(value: acCode))
            {
                a = a.Where(predicate: x => x.Pcode == acCode);
            }
            if (pageSize > 0 || pageNumber > 0)
            {
                a = a.Skip(count: pageNumber * pageSize).Take(count: pageSize);
            }

            var retx = await a.Select(selector: x => x.InvNo + "-" + x.Pcode)
             .ToArrayAsync(cancellationToken: ct);
            ret = await _db.InvSumms
                .Where(predicate: x => retx.Contains(x.InvNo + "-" + x.Pcode))
                .OrderBy(keySelector: x => x.InvNo)
                .Select(selector: x => new InvSummGridModel { InvNo = x.InvNo, Amount = x.TotBill + x.DisPer, NetAmount = x.TotBill, Name = x.Pname, PCode = x.Pcode })
                .ToArrayAsync(cancellationToken: ct);

            SetCache(key: key, value: ret);
        }

        return ret;
    }

    public async Task<SalesReportDto?> GetSalesInvoiceData(int invNo, string type, string acNumber, string pcode, CancellationToken ct)
    {
        var reportKey = "SR-" + invNo + "-" + type + "-" + acNumber + "-" + pcode;
        var successful = GetCache(key: reportKey, result: out SalesReportDto? ret);
        if (successful) return ret;
        ret = new SalesReportDto
        {
            Type = type,
        };

        var dataSummx = _db.InvSumms.AsNoTracking().Where(predicate: x => x.InvNo == invNo && x.Remarks == type && x.Pcode == pcode);
        if (!string.IsNullOrWhiteSpace(value: acNumber))
        {
            dataSummx = dataSummx.Where(predicate: x => x.Pcode == acNumber);
        }
        var dataSumm = dataSummx.Select(selector: x => new { x.TotBill, x.DisPer, x.Payment, x.RefNo, x.DueDate, x.InvDate, x.InvNo })
            .FirstOrDefault();
        if (dataSumm is not null)
        {
            ret.RefNumber = dataSumm.RefNo;
            ret.Dated = dataSumm.InvDate;
            ret.DueDate = dataSumm.DueDate;
            ret.Payment = dataSumm.Payment;
            ret.InvNo = dataSumm.InvNo ?? 0;
            ret.TotalBeforeDiscount = dataSumm.TotBill + dataSumm.DisPer;
            ret.TotalAfterDiscount = dataSumm.TotBill;
        }
        if (ret.InvNo == 0)
        {
            return ret;
        }
        ret.TableData ??= new List<SalesReportModel>();
        var r = _db.InvDets.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(value: acNumber))
        {
            r = r.Where(predicate: x => x.Pcode == acNumber);
        }
        ret.TableData.AddRange(
            collection: await r.Where(predicate: x => x.InvNo == invNo && x.Sp == type && x.Pcode == pcode)
                .Select(
                    selector: row => new SalesReportModel()
                    {
                        Amount = row.Amount,
                        NetAmount = row.NetAmount,
                        Rate = row.Rate,
                        Quantity = (int)(row.Qty ?? 0.0),
                        Unit = row.Unit,
                        Description = row.Iname,
                        Discount = row.Dper
                    }
                ).ToArrayAsync(cancellationToken: ct)
        );
        SetCache(key: reportKey, value: ret);

        return ret;
    }

    public async Task<bool> InsertAllDataBulk(string access, CancellationToken ct)
    {
        try
        {
            List<Acfile> acFileInsert = new();
            List<InvDet> invDetInsert = new();
            List<Inventory> inventoryInsert = new();
            List<InvSumm> invSummInsert = new();
            List<Trans> transInsert = new();
            var splitData = access.Split(separator: "---START---", options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Parallel.ForEach(source: splitData, body: splitEntries =>
            {
                var entries = splitEntries.Split(separator: new string[] { "\r\n", "\n" }, options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                var newEnt = entries.Skip(count: 1).Select(selector: x => x.Split(separator: "|"));
                switch (entries[0].ToUpperInvariant())
                {
                    case "ACFILE":
                        acFileInsert.AddRange(collection: newEnt.Select(selector: x => new Acfile(x: x)));
                        break;

                    case "INVDET":
                        invDetInsert.AddRange(collection: newEnt.Where(predicate: x => !string.IsNullOrWhiteSpace(value: x[0])).Select(selector: x => new InvDet(x: x)));
                        break;

                    case "INVENTORY":
                        inventoryInsert.AddRange(collection: newEnt.Select(selector: x => new Inventory(x: x)));
                        break;

                    case "INVSUMM":
                        invSummInsert.AddRange(collection: newEnt.Where(predicate: x => !string.IsNullOrWhiteSpace(value: x[1])).Select(selector: x => new InvSumm(x: x)));
                        break;

                    case "TRANS":
                        transInsert.AddRange(collection: newEnt.Select(selector: x => new Trans(x: x)));
                        break;
                }
            });
            try
            {
                await EnsureDbConnection(ct: ct);
                _logger.LogInformation(message: "Starting Insert");
                await _db.Database.EnsureDeletedAsync(cancellationToken: ct);
                await _db.Database.MigrateAsync(cancellationToken: ct);
                _logger.LogInformation(message: "Migration Done");
                await using (var transaction = await _db.Database.BeginTransactionAsync(cancellationToken: ct).ConfigureAwait(continueOnCapturedContext: false))
                {
                    try
                    {
                        string query;
                        foreach (var acfile in acFileInsert.Select(selector: x => x.ToInsert()).Chunk(size: 7000))
                        {
                            query = "INSERT INTO `acfile`(`ActCode`,`ActName`,`Address1`,`Address2`,`Address3`,`CrDays`,`email`,`fax`,`GST`,`OpBal`,`phone`) VALUES " +
                                string.Join(separator: ',', value: acfile) + ";";
                            await _db.Database.ExecuteSqlRawAsync(sql: query, cancellationToken: ct);
                        }

                        foreach (var invdet in invDetInsert.Select(selector: x => x.ToInsert()).Chunk(size: 7000))
                        {
                            query = "INSERT INTO `invdet`(`InvNo`,`InvDate`,`PCode`,`ICode`,`IName`,`Qty`,`Qty2`,`Unit`,`Packing`,`Rate`,`Amount`,`NetAmount`,`SP`,`Type`,`pName`,`Size`,`Pressure`,`CateCode`,`Dper`,`RegionCode`,`RegionName`,`FILE`) VALUES " +
                                string.Join(separator: ',', value: invdet) + ";";
                            await _db.Database.ExecuteSqlRawAsync(sql: query, cancellationToken: ct);
                        }

                        foreach (var invSumm in invSummInsert.Select(selector: x => x.ToInsert()).Chunk(size: 7000))
                        {
                            query = "INSERT INTO `invsumm`(`OrderNo`,`InvNo`,`InvDate`,`PCode`,`PName`, `TotBill`,`Built`,`DisPer`,`Dis`,`Ser`,`Remarks`,`cartage`,`AddLess`,`CrDays`,`DueDate`,`RefNo`,`Payment`,`Note`,`Delivery`,`HCode`) VALUES " +
                                string.Join(separator: ',', value: invSumm) + ";";
                            await _db.Database.ExecuteSqlRawAsync(sql: query, cancellationToken: ct);
                        }

                        foreach (var inventory in inventoryInsert.Select(selector: x => x.ToInsert()).Chunk(size: 7000))
                        {
                            query = "INSERT INTO `inventory`(`ItemCode`,`ItemDescrip`,`MfcCode`,`ManuName`,`Size`,`Pressure`,`Length`,`Price`,`RetPrice`,`RetPrice2`,`Unit`,`OpBal`) VALUES " +
                                string.Join(separator: ',', value: inventory) + ";";
                            await _db.Database.ExecuteSqlRawAsync(sql: query, cancellationToken: ct);
                        }

                        foreach (var trans in transInsert.Select(selector: x => x.ToInsert()).Chunk(size: 7000))
                        {
                            query = "INSERT INTO `trans`(`ActCode`,`ActName`,`ChqDate`,`ChqNo`,`Date`,`Des`,`TransAmt`,`Vnoc`,`Vnon`) VALUES " +
                                string.Join(separator: ',', value: trans) + ";";
                            await _db.Database.ExecuteSqlRawAsync(sql: query, cancellationToken: ct);
                        }
                        await transaction.CommitAsync(cancellationToken: ct).ConfigureAwait(continueOnCapturedContext: false);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(message: ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
#endif
                        await transaction.RollbackAsync(cancellationToken: ct).ConfigureAwait(continueOnCapturedContext: false);
                        return false;
                    }
                }
                await _db.Database.CloseConnectionAsync();
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(message: ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
#endif
                return false;
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(message: ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
#endif
            return false;
        }
    }

    public async Task<AccountInfoDbContext> GetDbContext(CancellationToken ct)
    {
        await EnsureDbConnection(ct: ct);
        return _db;
    }

    private async Task EnsureDbConnection(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value: _dbName))
        {
            throw new DbEmptyException(message: "No database name set");
        }
        if (string.IsNullOrWhiteSpace(value: ConnectionString))
        {
            throw new DbEmptyException(message: "No connection string set");
        }
        var db = _db.Database.GetDbConnection().Database;
        if (db != _dbName || !await _db.Database.CanConnectAsync(cancellationToken: ct))
        {
            _db.Database.SetConnectionString(connectionString: ConnectionString);
        }
    }

    private void SetCache<T>(string key, T value)
    {
        if (_cache is not null)
        {
            _cache.Set(key: CachePrefix + key, value: value);
        }
    }

    private bool GetCache<T>(string key, out T result)
    {
        result = default!;
        if (_cache is not null)
        {
            return _cache.TryGetValue(key: CachePrefix + key, value: out result);
        }
        return false;
    }
}