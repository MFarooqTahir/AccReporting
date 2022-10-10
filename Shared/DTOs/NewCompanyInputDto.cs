using System.ComponentModel.DataAnnotations;

namespace AccReporting.Shared.DTOs;

public class NewCompanyInputDto
{
    [StringLength(maximumLength: 30, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 30 characters")]
    public string Name { get; set; }

    [StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "Address must be between 1 and 50 characters")]
    public string Address { get; set; }

    [DataType(dataType: DataType.PhoneNumber, ErrorMessage = "Phone number is not valid")]
    public string Phone { get; set; }
}