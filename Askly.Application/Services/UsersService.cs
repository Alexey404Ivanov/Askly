using Askly.Application.DTOs.Users;
using Askly.Application.Interfaces.Auth;
using Askly.Application.Interfaces.Repositories;
using Askly.Application.Interfaces.Services;
using Askly.Domain;
using AutoMapper;

namespace Askly.Application.Services;

public class UsersService : IUsersService
{
    private readonly IPasswordHasher _hasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMapper _mapper;
    
    public UsersService(IPasswordHasher hasher, IUsersRepository usersRepository, IJwtProvider jwtProvider, IMapper mapper)
    {
        _hasher = hasher;
        _usersRepository = usersRepository;
        _jwtProvider = jwtProvider;
        _mapper = mapper;
    }
    
    public async Task Register(string userName, string email, string password)
    {
        var hashedPassword = _hasher.HashPassword(password);
        var user = UserEntity.Create(userName, email, hashedPassword);
        await _usersRepository.Add(user);
    }

    public async Task<string> Login(string email, string password)
    {
        var user = await _usersRepository.GetByEmail(email);
        if (user == null)
            throw new Exception("User not found");
        var isPasswordValid = _hasher.VerifyPassword(password, user.HashedPassword);
        if (!isPasswordValid)
        {
            throw new Exception("Invalid password");
        }
        
        var token = _jwtProvider.GenerateToken(user);
        return token;
    }

    public async Task<UserProfileDto> GetUserProfileInfo(Guid userId)
    {
        var user = await _usersRepository.GetById(userId);
        return _mapper.Map<UserProfileDto>(user);
    }
}