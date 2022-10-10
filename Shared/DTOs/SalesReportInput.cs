using System.ComponentModel.DataAnnotations;

namespace AccReporting.Shared.DTOs
{

    public class SalesReportInput
    {
        [Required(ErrorMessage = "Invoice number not given"), Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Invoice number out of range")]
        public int? InvNo { get; set; }

        [Required(ErrorMessage = "Type not given")]
        public string? Type { get; set; }
    }
}