namespace Askly.Application.Interfaces.Repositories;

public interface IVotesRepository
{
    Task<List<Guid>> GetUserVotedOptionIds(Guid pollId, Guid userId);
    Task Vote(Guid pollId, Guid[] optionIds, Guid userId);
    Task DeleteVote(Guid pollId, Guid userId);
    Task<int> GetVotedUsersCount(Guid pollId);
    Task<List<Tuple<Guid, int>>> GetResults(Guid pollId);
}