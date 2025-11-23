using VoitingApp.Domain;

namespace VoitingApp.Infrastructure;

public interface IPolesRepository
{
    PoleEntity Insert(PoleEntity pole);
    PoleEntity? FindById(Guid poleId);
    IEnumerable<PoleEntity> GetAll();
    void Delete(Guid poleId);
    void UpdateVotes(Guid poleId, List<Guid> optionsIds, bool isVoteReset);
}