using Askly.Domain;

namespace Askly.Application.Interfaces.Repositories;

public interface IUsersRepository
{
    Task Add(UserEntity user);
    Task<UserEntity?> GetByEmail(string email);
    Task<UserEntity?> GetById(Guid userId);
}