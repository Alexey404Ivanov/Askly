using Askly.Application.DTOs.Polls;
using Askly.Application.Exceptions;
using Askly.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Askly.Api.Controllers.Polls;

[ApiController]
[Route("api/polls")]
public class PollsApiController : ControllerBase
{
    private readonly IPollsService _service;

    public PollsApiController(IPollsService service)
    {
        _service = service;
    }
    
    [Authorize]
    [HttpGet("{pollId:guid}", Name = nameof(GetById))]
    [Produces("application/json")]
    public async Task<ActionResult<PollDto>> GetById([FromRoute] Guid pollId)
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        var poll = await _service.GetById(pollId, userId);
        return Ok(poll);
    }
    
    [HttpGet]
    [Produces("application/json")]
    public async Task<ActionResult<List<PollDto>>> GetAll()
    {
        var polls = await _service.GetAll();
        return Ok(polls);
    }
    
    [Authorize]
    [HttpPost]
    [Produces("application/json", "application/xml")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePollDto pollDto)
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        var options = pollDto.Options.Select(optionDto => optionDto.Text).ToArray();
        var createdPollId = await _service.Create(pollDto.Title, options, (bool)pollDto.IsMultipleChoice!, userId);
        return Ok(createdPollId);
    }
    
    [Authorize]
    [HttpPost("{pollId:guid}/vote")]
    [Produces("application/json")]
    public async Task<IActionResult> Vote([FromRoute] Guid pollId, [FromBody] CreateVoteDto voteDto)
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);

        await _service.Vote(pollId, voteDto.OptionIds, userId);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{pollId:guid}")]
    public async Task<ActionResult> DeletePoll([FromRoute] Guid pollId)
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        await _service.DeletePoll(pollId, userId);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{pollId:guid}/vote")]
    public async Task<ActionResult> DeleteVote([FromRoute] Guid pollId)
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        await _service.DeleteVote(pollId, userId);
        return NoContent();
    }


    [Authorize]
    [HttpGet("{pollId:guid}/results")]
    [Produces("application/json")]
    public async Task<ActionResult<List<VoteResultsDto>>> ShowResults([FromRoute] Guid pollId)
    {
        var resultsDto = await _service.GetResults(pollId);
        return Ok(resultsDto);
    }
}
