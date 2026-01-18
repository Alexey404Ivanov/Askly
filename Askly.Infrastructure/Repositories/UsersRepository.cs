using Askly.Application.Interfaces.Repositories;
using Askly.Domain;
using Microsoft.EntityFrameworkCore;

namespace Askly.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _context;

    public UsersRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task Add(UserEntity user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserEntity?> GetByEmail(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<UserEntity?> GetById(Guid userId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    public async Task UpdateUserInfo(Guid userId, string name, string email)
    {
        await _context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(user => user.Name, name)
                .SetProperty(user => user.Email, email));
        
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateUserPassword(Guid userId, string newHashedPassword)
    {
        await _context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(user => user.HashedPassword, newHashedPassword));
        
        await _context.SaveChangesAsync();
    }
}