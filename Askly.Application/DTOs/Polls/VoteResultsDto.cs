namespace Askly.Application.DTOs.Polls;

public class VoteResultsDto
{
    public Guid OptionId { get; init; }
    public int VotesCount { get; init; }
    public double Ratio { get; init; }
}