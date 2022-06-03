using AccountsReportsWASM.Shared.ReportModels;

using System.ComponentModel.DataAnnotations;

namespace AccReporting.Shared.DTOs
{
    public class SalesReportDto
    {
        public List<SalesReportModel> tableData { get; set; }
        public double Total { get; set; }
        public string Payment { get; set; }
        public string Type { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string cell { get; set; }
        public int InvNo { get; set; }
        public DateTime? Dated { get; set; }
        public DateTime? DueDate { get; set; }
        public string RefNumber { get; set; }
        public string Driver { get; set; }
    }

    public class SalesReportInput
    {
        [Required(ErrorMessage = "Invoice number not given"), Range(1, int.MaxValue, ErrorMessage = "Invoice number out of range")]
        public int? InvNo { get; set; }

        [Required(ErrorMessage = "Type not given")]
        public string? Type { get; set; }
    }
}