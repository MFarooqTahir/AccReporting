using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccReporting.Shared.ContextModels
{
    public class CompanyAccount
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(length: 20)]
        public string? CompRole { get; set; }

        [MaxLength(length: 15)]
        public string? AcNumber { get; set; }

        [DefaultValue(value: false)]
        public bool IsSelected { get; set; }

        [ForeignKey(name: nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser? User { get; set; }

        [ForeignKey(name: nameof(Company))]
        public int? CompanyId { get; set; }

        public CompanyDetail? Company { get; set; }
    }
}