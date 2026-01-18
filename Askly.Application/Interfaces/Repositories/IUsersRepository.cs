using Askly.Domain;

namespace Askly.Application.Interfaces.Repositories;

public interface IUsersRepository
{
    Task Add(UserEntity user);
    Task<UserEntity?> GetByEmail(string email);
    Task<UserEntity?> GetById(Guid userId);
    Task UpdateUserInfo(Guid userId, string name, string email);
    Task UpdateUserPassword(Guid userId, string newHashedPassword);
}