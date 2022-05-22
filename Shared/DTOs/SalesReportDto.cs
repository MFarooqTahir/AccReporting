using AccountsReportsWASM.Shared.ReportModels;

namespace AccReporting.Shared.DTOs
{
    public class SalesReportDto
    {
        public IEnumerable<SalesReportModel> tableData { get; set; }
        public double Total { get; set; }
        public string NameAndAddress { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string cell { get; set; }
        public int InvNo { get; set; }
        public DateTimeOffset Dated { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string RefNumber { get; set; }
        public string Driver { get; set; }
    }
}