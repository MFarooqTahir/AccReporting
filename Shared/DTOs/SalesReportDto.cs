using AccountsReportsWASM.Shared.ReportModels;

namespace AccReporting.Shared.DTOs
{
    public class SalesReportDto
    {
        public List<SalesReportModel> TableData { get; set; }
        public double? TotalBeforeDiscount { get; set; }
        public double? TotalAfterDiscount { get; set; }
        public string Payment { get; set; }
        public string Type { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Cell { get; set; }
        public int InvNo { get; set; }
        public DateTime? Dated { get; set; }
        public DateTime? DueDate { get; set; }
        public string RefNumber { get; set; }
        public string Driver { get; set; }
    }
}