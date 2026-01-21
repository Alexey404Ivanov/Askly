using Askly.Application.DTOs.Polls;

namespace Askly.Application.Interfaces.Services;

public interface IPollsService
{
    Task<PollDto> GetById(Guid pollId, Guid userId);
    Task<Guid> Create(string title, string[] options, bool isMultipleChoice, Guid userId);
    Task<List<PollDto>> GetAll();
    Task DeletePoll(Guid pollId, Guid userId);
    // Task Vote(Guid id, List<Guid> optionsIds);
    Task Vote(Guid pollId, Guid[] optionsIds, Guid userId);
    Task DeleteVote(Guid pollId, Guid userId);
    Task<List<VoteResultsDto>> GetResults(Guid pollId);
    // void DeleteVote(Guid pollId, List<Guid> optionsIds);
    // PollResultsDto ShowResults(Guid pollId);
}