using Askly.Application.DTOs.Users;

namespace Askly.Application.Interfaces.Services;

public interface IUsersService
{
    Task Register(string name, string email, string password);
    Task<string> Login(string email, string password);
    Task<UserProfileDto> GetUserProfileInfo(Guid userId);
    Task UpdateUserInfo(Guid userId, string name, string email);
    Task UpdateUserPassword(Guid userId, string currentPassword, string newPassword);
}