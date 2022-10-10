namespace AccReporting.Shared.DTOs
{
    public class CompaniesListDto
    {
        public CompaniesListDto(string id, string compId, string name, string? accountNo, string role, bool isSelected)
        {
            this.Id = id;
            this.CompId = compId;
            Name = name;
            AccountNo = accountNo;
            Role = role;
            IsSelected = isSelected;
        }

        public CompaniesListDto()
        {
        }

        public string Id { get; set; }
        public string CompId { get; set; }
        public string Name { get; set; }
        public string? AccountNo { get; set; }
        public string Role { get; set; }
        public bool IsSelected { get; set; }
    }
}