using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Polls;

public class CreatePollDto
{
    [Required(ErrorMessage = "Необходимо задать название опроса")]
    [MinLength(10, ErrorMessage = "Недопустимая длина названия опроса. Минимум 10 символов")]
    [MaxLength(75, ErrorMessage = "Недопустимая длина названия опроса. Максимум 75 символов")]
    public string Title { get; init; }
    
    [Required(ErrorMessage = "Нет возможных вариантов ответа")]
    [MinLength(2, ErrorMessage = "Недопустимое количество вариантов ответа. Минимум 2")]
    [MaxLength(10, ErrorMessage = "Недопустимое количество вариантов ответа. Максимум 10")]
    public CreateOptionDto[] Options { get; init; }
    
    [Required(ErrorMessage = "Нельзя определить тип опроса")]
    public bool? IsMultipleChoice { get; init; }
}