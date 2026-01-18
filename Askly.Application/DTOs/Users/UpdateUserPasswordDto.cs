using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Users;

public class UpdateUserPasswordDto
{
    [Required]
    public string CurrentPassword { get; set; }
    [Required]
    public string NewPassword { get; set; }
}