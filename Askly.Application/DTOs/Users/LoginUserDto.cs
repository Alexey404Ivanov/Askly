using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Users;

public class LoginUserDto
{
    [Required(ErrorMessage = "Необходимо указать емэйл")]
    public string Email { get; init; }
    
    [Required(ErrorMessage = "Необходимо указать пароль")]
    public string Password { get; init; }
}