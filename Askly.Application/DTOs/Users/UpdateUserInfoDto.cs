using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Users;

public class UpdateUserInfoDto
{
    [Required(ErrorMessage = "Необходимо задать имя")]
    [MinLength(2, ErrorMessage = "Недопустимая длина имени. Минимум 2 буквы")]
    [MaxLength(30, ErrorMessage = "Недопустимая длина имени. Максимум 30 буквы")]
    public string Name { get; set; } 
    
    [Required(ErrorMessage = "Необходимо задать емэйл")]
    [EmailAddress(ErrorMessage = "Емэйл введён в неверном формате")]
    public string Email { get; set; }
}