using Askly.Application.Interfaces.Repositories;
using Askly.Domain;
using Microsoft.EntityFrameworkCore;

namespace Askly.Infrastructure.Repositories;

public class VotesRepository : IVotesRepository
{
    private readonly AppDbContext _context;
    
    public VotesRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Guid>> GetUserVotedOptionIds(Guid pollId, Guid userId)
    {
        return await _context.Votes
            .Where(v => v.PollId == pollId && v.UserId == userId)
            .Select(v => v.OptionId)
            .ToListAsync();
    }

    public async Task Vote(Guid pollId, Guid[] optionIds, Guid userId)
    {
        
        await _context.Votes
            .Where(v => v.PollId == pollId && v.UserId == userId)
            .ExecuteDeleteAsync();
        
        var votes = optionIds
            .Select(optionId => 
                VoteEntity.Create(userId, pollId, optionId));
        
        await _context.Votes.AddRangeAsync(votes);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteVote(Guid pollId, Guid userId)
    {
        await _context.Votes
            .Where(v => v.PollId == pollId && v.UserId == userId)
            .ExecuteDeleteAsync();
        
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetVotedUsersCount(Guid pollId)
    {
        return await _context.Votes
            .AsNoTracking()
            .Where(v => v.PollId == pollId)
            .Select(v => v.UserId)
            .Distinct()
            .CountAsync();
    }
    public async Task<List<Tuple<Guid, int>>> GetResults(Guid pollId)
    {
        return await _context.Votes
            .AsNoTracking()
            .Where(v => v.PollId == pollId)
            .GroupBy(v => v.OptionId)
            .Select(g => new Tuple<Guid, int>(g.Key, g.Count()))
            .ToListAsync();
    }
}