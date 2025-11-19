using VoitingApp.Domain;

namespace VoitingApp.Models;

public class PoleDto
{
    public Guid Id { get; set; }
    public string Question { get; set; }
    public List<OptionDto> Options { get; set; }
}