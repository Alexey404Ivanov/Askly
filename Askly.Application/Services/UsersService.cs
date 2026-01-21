using Askly.Application.DTOs.Users;
using Askly.Application.Exceptions;
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
    
    public async Task Register(string name, string email, string password)
    {
        var existingUser = await _usersRepository.GetByEmail(email);
        if (existingUser != null)
            throw new UserAlreadyExistsException();
        
        var hashedPassword = _hasher.HashPassword(password);
        var user = UserEntity.Create(name, email, hashedPassword);
        await _usersRepository.Add(user);
    }

    public async Task<string> Login(string email, string password)
    {
        var user = await _usersRepository.GetByEmail(email);
        if (user == null)
            throw new UserNotFoundException(email);
        
        var isPasswordValid = _hasher.VerifyPassword(user.HashedPassword, password);
        if (!isPasswordValid)
            throw new InvalidPasswordException();
        
        var token = _jwtProvider.GenerateToken(user);
        return token;
    }

    public async Task<UserProfileDto> GetUserProfileInfo(Guid userId)
    {
        var user = await _usersRepository.GetById(userId);
        return _mapper.Map<UserProfileDto>(user);
    }
    
    public async Task UpdateUserInfo(Guid userId, string name, string email)
    {
        await _usersRepository.UpdateUserInfo(userId, name, email);
    }
    
    public async Task UpdateUserPassword(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _usersRepository.GetById(userId);
        
        var isPasswordValid = _hasher.VerifyPassword(user!.HashedPassword, currentPassword);
        if (!isPasswordValid)
            throw new InvalidPasswordException();
        
        var hashedPassword = _hasher.HashPassword(newPassword);
        await _usersRepository.UpdateUserPassword(userId, hashedPassword);
    }
}