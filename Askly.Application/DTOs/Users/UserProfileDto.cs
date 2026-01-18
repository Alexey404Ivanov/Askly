using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Users;

public class UserProfileDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
}