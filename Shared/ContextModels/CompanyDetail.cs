using System.ComponentModel.DataAnnotations;

namespace AccReporting.Shared.ContextModels
{
    public class CompanyDetail
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(30)]
        public string? Name { get; set; }

        [MaxLength(30)]
        public string? DbName { get; set; }

        [MaxLength(150)]
        public string? Address { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        public ICollection<CompanyAccount>? CompanyUsers { get; set; }
        public ICollection<ApplicationUser>? CurrentSelectedUsers { get; set; }
    }
}