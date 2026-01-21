using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Users;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Необходимо задать имя")]
    [MinLength(2, ErrorMessage = "Недопустимая длина имени. Минимум 2 буквы")]
    [MaxLength(30, ErrorMessage = "Недопустимая длина имени. Максимум 30 буквы")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Необходимо задать емэйл")]
    [EmailAddress(ErrorMessage = "Емэйл введён в неверном формате")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Необходимо задать пароль")]
    [MinLength(5, ErrorMessage = "Недопустимая длина пароля. Минимум 5 символа")]
    [MaxLength(30, ErrorMessage = "Недопустимая длина имени. Максимум 30 символов")]
    public string Password { get; set; }
}