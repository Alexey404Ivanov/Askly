using VoitingApp.Infrastructure;

namespace VoitingApp.Domain;

public class PolesRepository : IPolesRepository
{
    private readonly Dictionary<Guid, PoleEntity> _entities = new();

    public PoleEntity Insert(PoleEntity pole)
    {
        if (pole.Id != Guid.Empty)
            throw new InvalidOperationException();
        var id = Guid.NewGuid();
        var entity = Clone(id, pole);
        InitOptionsId(entity);
        _entities[id] = entity;
        return Clone(id, entity);
    }

    public PoleEntity? FindById(Guid poleId)
    {
        return _entities.TryGetValue(poleId, out var entity) ? Clone(poleId, entity) : null;
    }

    public void UpdateVotes(Guid poleId, List<Guid> optionsIds, bool isVoteReset)
    {
        var pole = _entities[poleId];
        foreach (var option in pole.Options.Where(option => optionsIds.Contains(option.Id)))
        {
            if (!isVoteReset) option.VotesCount++;
            else option.VotesCount--;
        }
    }

    public IEnumerable<PoleEntity> GetAll()
    {
        return _entities.Values.Select(x => Clone(x.Id, x));
    }

    public void Delete(Guid poleId)
    {
        _entities.Remove(poleId);
    }

    private static void InitOptionsId(PoleEntity pole)
    {
        foreach (var option in pole.Options)
        {
            option.Id = Guid.NewGuid();
            option.PoleId = pole.Id;
        }
    }
    
    private static PoleEntity Clone(Guid id, PoleEntity pole)
    {
        return new PoleEntity(id, pole.Question, pole.CreatedAt, pole.Options, pole.IsMultipleChoice);
    }
}