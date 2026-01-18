using Askly.Application.DTOs.Users;

namespace Askly.Application.Interfaces.Services;

public interface IUsersService
{
    Task Register(string userName, string email, string password);
    Task<string> Login(string email, string password);
    Task<UserProfileDto> GetUserProfileInfo(Guid userId);
    Task UpdateUserInfo(Guid userId, UpdateUserInfoDto updateDto);
    Task UpdateUserPassword(Guid userId, UpdateUserPasswordDto updateDto);
}