using System.ComponentModel.DataAnnotations;

namespace AccReporting.Shared.ContextModels
{
    public class CompanyDetail
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(length: 30)]
        public string? Name { get; set; }

        [MaxLength(length: 30)]
        public string? DbName { get; set; }

        [MaxLength(length: 150)]
        public string? Address { get; set; }

        [MaxLength(length: 15)]
        public string? Phone { get; set; }
        public bool Approved { get; set; }
        public ICollection<CompanyAccount>? CompanyUsers { get; set; }
        public ICollection<ApplicationUser>? CurrentSelectedUsers { get; set; }
    }
}