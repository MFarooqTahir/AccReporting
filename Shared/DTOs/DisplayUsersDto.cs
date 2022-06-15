namespace AccReporting.Shared.DTOs;

public class DisplayUsersDto
{
    public int? ID { get; set; }
    public string? UserEmail { get; set; }
    public string AcCode { get; set; }
    public bool IsAdmin { get; set; }
}