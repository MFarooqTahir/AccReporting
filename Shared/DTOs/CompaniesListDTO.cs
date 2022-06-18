namespace AccReporting.Shared.DTOs
{
    public class CompaniesListDTO
    {
        public CompaniesListDTO(string ID, string CompID, string name, string? accountNo, string role, bool isSelected)
        {
            this.ID = ID;
            this.CompID = CompID;
            Name = name;
            AccountNo = accountNo;
            Role = role;
            IsSelected = isSelected;
        }

        public CompaniesListDTO()
        {
        }

        public string ID { get; set; }
        public string CompID { get; set; }
        public string Name { get; set; }
        public string? AccountNo { get; set; }
        public string Role { get; set; }
        public bool IsSelected { get; set; }
    }
}