using Askly.Application.Interfaces.Repositories;
using Askly.Domain;
using Askly.Domain.Entities;

namespace Askly.Infrastructure.Repositories;

public class InMemoryPollsRepository : IPollsRepository
{
    private readonly Dictionary<Guid, PollEntity> _entities = new();

    public PollEntity Create(PollEntity poll)
    {
        if (poll.Id != Guid.Empty)
            throw new InvalidOperationException();
        var id = Guid.NewGuid();
        var entity = Clone(id, poll);
        InitOptionsId(entity);
        _entities[id] = entity;
        return Clone(id, entity);
    }

    public PollEntity? FindById(Guid pollId)
    {
        return _entities.TryGetValue(pollId, out var entity) ? Clone(pollId, entity) : null;
    }

    public void UpdateVotes(Guid pollId, List<Guid> optionsIds, bool isVoteReset)
    {
        var pole = _entities[pollId];
        foreach (var option in pole.Options.Where(option => optionsIds.Contains(option.Id)))
        {
            // if (!isVoteReset) option.VotesCount++;
            // else option.VotesCount--;
        }
    }

    public IEnumerable<PollEntity> GetAll()
    {
        return _entities.Values.Select(x => Clone(x.Id, x));
    }

    public void Delete(Guid pollId)
    {
        _entities.Remove(pollId);
    }

    private static void InitOptionsId(PollEntity poll)
    {
        foreach (var option in poll.Options)
        {
            option.Id = Guid.NewGuid();
            option.PollId = poll.Id;
        }
    }
    
    private static PollEntity Clone(Guid id, PollEntity poll)
    {
        return new PollEntity(id, poll.Title, poll.CreatedAt, poll.Options, poll.IsMultipleChoice);
    }
}