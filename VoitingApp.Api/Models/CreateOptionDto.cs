using System.ComponentModel.DataAnnotations;

namespace VoitingApp.Models;

public class CreateOptionDto
{
    [Required]
    public string Text { get; set; }
}