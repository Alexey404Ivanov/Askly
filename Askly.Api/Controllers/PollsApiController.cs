using Microsoft.AspNetCore.Mvc;
using Askly.Application.Interfaces.Repositories;
using Askly.Application.Services;
using Askly.Application.DTOs;
using Askly.Application.Exceptions;
namespace Askly.Api.Controllers;

[ApiController]
[Route("api/polls")]
public class PollsApiController : ControllerBase
{
    private readonly IPollService _service;

    public PollsApiController(IPollsRepository repo, IPollService service)
    {
        _service = service;
    }
    
    [HttpGet("{pollId:guid}", Name = nameof(GetById))]
    [Produces("application/json")]
    public ActionResult<PollDto> GetById([FromRoute] Guid pollId)
    {
        try
        {
            var poll = _service.GetById(pollId);
            return Ok(poll);
        }
        catch (PollNotFoundException e)
        {
            return NotFound(e.Message);
        }
        
        // var pole = _repo.FindById(poleId);
        // if (pole == null)
        //     return NotFound();
        // var dto = _mapper.Map<PoleDto>(pole);
        // return Ok(dto);
    }
    
    [HttpGet]
    [Produces("application/json")]
    public ActionResult<IEnumerable<PollDto>> GetAll()
    {
        var polls = _service.GetAll();
        return Ok(polls);
    }
    
    [HttpPost]
    [Produces("application/json", "application/xml")]
    public ActionResult<Guid> Create([FromBody] CreatePollDto? pollDto)
    {
        if (pollDto == null)
            return BadRequest();
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        var createdPollId = _service.Create(pollDto);
        return Ok(createdPollId);
    }
    
    
    [HttpPost("{pollId:guid}/vote")]
    [Produces("application/json")]
    public ActionResult Vote([FromRoute] Guid pollId, [FromBody] List<Guid> optionsIds)
    {
        try
        {
            _service.Vote(pollId, optionsIds);
            return NoContent();
        }
        catch (PollNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpDelete("{pollId:guid}")]
    public ActionResult DeletePoll([FromRoute] Guid pollId)
    {
        try
        {
            _service.DeletePoll(pollId);
            return NoContent();
        }
        catch (PollNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpDelete("{pollId:guid}/vote")]
    [Produces("application/json")]
    public ActionResult DeleteVote([FromRoute] Guid pollId, [FromBody] List<Guid> optionsIds)
    {
        try
        {
            _service.DeleteVote(pollId, optionsIds);
            return NoContent();
        }
        catch (PollNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    

    [HttpGet("{pollId:guid}/results")]
    [Produces("application/json")]
    public ActionResult<PollResultsDto> ShowResults([FromRoute] Guid pollId)
    {
        try
        {
            var resultsDto = _service.ShowResults(pollId);
            return Ok(resultsDto);
        }
        catch (PollNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
