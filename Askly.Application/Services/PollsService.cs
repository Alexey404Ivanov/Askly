using System.Collections.Frozen;
using Askly.Application.Interfaces.Repositories;
using Askly.Application.DTOs.Polls;
using Askly.Domain;
using Askly.Application.Exceptions;
using Askly.Application.Interfaces.Services;
using AutoMapper;

namespace Askly.Application.Services;

public class PollsService : IPollsService
{
    private readonly IPollsRepository _pollsRepository;
    private readonly IVotesRepository _votesRepository;
    private readonly IMapper _mapper;
    
    public PollsService(
        IPollsRepository pollsRepository,
        IVotesRepository votesRepository,
        IMapper mapper)
    {
        _pollsRepository = pollsRepository;
        _votesRepository = votesRepository;
        _mapper = mapper;
    }
    
    public async Task<PollDto> GetById(Guid pollId, Guid userId)
    {
        var poll = await _pollsRepository.GetById(pollId);
        if (poll == null)
            throw new PollNotFoundException(pollId);
        
        var votedOptions = await _votesRepository.GetUserVotedOptionIds(pollId, userId);
        
        var dto = _mapper.Map<PollDto>(poll);
        dto.UserVotes = votedOptions;
        return dto;
    }
    
    public async Task<Guid> Create(
        string title,
        string[] options,
        bool isMultipleChoice,
        Guid userId)
    {
        var poll = PollEntity.Create(title, isMultipleChoice, userId);
        
        foreach (var option in options)
            poll.AddOption(option);
        
        return await _pollsRepository.Add(poll);
    }
    
    public async Task<List<PollDto>> GetAll()
    {
        var polls = await _pollsRepository.GetAll();
        return _mapper.Map<List<PollDto>>(polls);
    }
    
    public async Task DeletePoll(Guid pollId, Guid userId)
    {
        var poll = await _pollsRepository.GetById(pollId);
        if (poll == null)
            throw new PollNotFoundException(pollId);
        
        if (poll.UserId != userId)
            throw new ForbiddenException(pollId);
        
        await _pollsRepository.Delete(pollId);
    }
    
    public async Task Vote(Guid pollId, Guid[] optionsIds, Guid userId)
    {
        var poll = await _pollsRepository.GetById(pollId);
        if (poll == null)
            throw new PollNotFoundException(pollId);
        
        var pollOptionsIds = poll.Options.Select(o => o.Id).ToFrozenSet();
        var notFoundOptions = optionsIds.Where(id => !pollOptionsIds.Contains(id)).ToArray();
        if (notFoundOptions.Length != 0)
            throw new PollOptionsNotFoundException(notFoundOptions);
        
        await _votesRepository.Vote(pollId, optionsIds, userId);
    }

    public async Task DeleteVote(Guid pollId, Guid userId)
    {
        var poll = await _pollsRepository.GetById(pollId);
        if (poll == null)
            throw new PollNotFoundException(pollId);
        
        var userVotes = await _votesRepository.GetUserVotedOptionIds(pollId, userId);
        if (userVotes.Count == 0)
            throw new VoteNotFoundException();
        
        await _votesRepository.DeleteVote(pollId, userId);
    }
    
    public async Task<List<VoteResultsDto>> GetResults(Guid pollId)
    {
        var poll = await _pollsRepository.GetById(pollId);
        if (poll == null)
            throw new PollNotFoundException(pollId);
        
        var votedOptions = await _votesRepository.GetResults(pollId);
        var votedUsersCount = await _votesRepository.GetVotedUsersCount(pollId);
        var allOptionGuids = (await _pollsRepository.GetById(pollId))!.Options.Select(x => x.Id).ToList();
        var votedOptionGuids = votedOptions.Select(t => t.Item1).ToHashSet();
        var results = new List<VoteResultsDto>();
        foreach (var optionGuid in allOptionGuids)
        {
            if (!votedOptionGuids.Contains(optionGuid))
                results.Add(new VoteResultsDto
                {
                    OptionId = optionGuid,
                    VotesCount = 0,
                    Ratio = 0
                });
            else 
            {
                var tuple = votedOptions.First(t => t.Item1 == optionGuid);
                results.Add(new VoteResultsDto
                {
                    OptionId = tuple.Item1,
                    VotesCount = tuple.Item2,
                    Ratio = Math.Round((double)tuple.Item2 / votedUsersCount * 100, 1)
                });
            }
        }
        return results;
    }
}