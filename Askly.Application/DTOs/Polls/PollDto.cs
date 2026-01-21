namespace Askly.Application.DTOs.Polls;

public class PollDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public List<OptionDto> Options { get; init; }
    public bool IsMultipleChoice { get; init; }
    public List<Guid> UserVotes { get; set; }
    public Guid UserId { get; init; }
}