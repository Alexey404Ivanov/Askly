namespace VoitingApp.Models;

public class PoleResultsDto
{
    public Guid Id { get; set; }
    public string Question { get; set; }
    public List<OptionResultsDto> Options { get; set; }
}