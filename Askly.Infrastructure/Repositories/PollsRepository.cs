using Askly.Application.Interfaces.Repositories;
using Askly.Domain;
using Microsoft.EntityFrameworkCore;

namespace Askly.Infrastructure.Repositories;

public class PollsRepository : IPollsRepository
{
    private readonly AppDbContext _context;
    
    public PollsRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<PollEntity>> GetAll()
    {
        var polls = await _context.Poles
            .AsNoTracking()
            .Include(p => p.Options)
            .ToListAsync();
        
        return polls;
    }
    
    public async Task<PollEntity?> GetById(Guid pollId)
    {
        var poll = await _context.Poles
            .AsNoTracking()
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == pollId);

        return poll;
    }

    public async Task<Guid> Add(PollEntity poll)
    {
        _context.Poles.Add(poll);
        await _context.SaveChangesAsync();
        
        return poll.Id;
    }

    public async Task Delete(Guid pollId)
    {
        var poll = await _context.Poles
            .FirstOrDefaultAsync(p => p.Id == pollId);
        
        _context.Poles.Remove(poll!);
        await _context.SaveChangesAsync();
    }
}