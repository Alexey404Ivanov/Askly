using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Users;

public class UpdateUserPasswordDto
{
    [Required(ErrorMessage = "Необходимо задать старый пароль")]
    public string CurrentPassword { get; set; }
    
    [Required(ErrorMessage = "Необходимо задать новый пароль")]
    [MinLength(5, ErrorMessage = "Недопустимая длина пароля. Минимум 5 символа")]
    [MaxLength(30, ErrorMessage = "Недопустимая длина пароля. Максимум 30 символов")]
    public string NewPassword { get; set; }
}