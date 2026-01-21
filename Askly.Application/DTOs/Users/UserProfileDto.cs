using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Users;

public class UserProfileDto
{
    [Required]
    public string Name { get; init; }
    [Required]
    public string Email { get; init; }
}