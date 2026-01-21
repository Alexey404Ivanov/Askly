using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Polls;

public class CreateOptionDto
{
    [Required(ErrorMessage = "Необходимо задать текст варианта ответа")]
    [MaxLength(50, ErrorMessage = "Недопустимая длина ответа. Максимум 50 символов")]
    public string Text { get; init; }
}