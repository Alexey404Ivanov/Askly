using System.ComponentModel.DataAnnotations;

namespace Askly.Application.DTOs.Polls;

public class CreateVoteDto
{
    [Required(ErrorMessage = "Необходимо указать идентификаторы выбранных вариантов ответа")]
    [MinLength(1, ErrorMessage = "Недопустимое количество выбранных вариантов ответа. Минимум 1")]
    public Guid[] OptionIds { get; set; }
}