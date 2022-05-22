using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccReporting.Shared.ContextModels
{
    public class CompanyAccount
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(20)]
        public string? CompRole { get; set; }

        [MaxLength(15)]
        public string? AcNumber { get; set; }

        [DefaultValue(false)]
        public bool? IsSelected { get; set; }

        [ForeignKey(nameof(User))]
        public string UserID { get; set; }

        public ApplicationUser? User { get; set; }

        [ForeignKey(nameof(Company))]
        public int? CompanyID { get; set; }

        public CompanyDetail? Company { get; set; }
    }
}