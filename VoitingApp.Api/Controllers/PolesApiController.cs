using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VoitingApp.Domain;
using VoitingApp.Infrastructure;
using VoitingApp.Models;

namespace VoitingApp.Controllers;

[ApiController]
[Route("api/poles")]
public class PolesApiController : ControllerBase
{
    private readonly IPolesRepository _repo;
    private readonly IMapper _mapper;

    public PolesApiController(IPolesRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    
    [HttpGet("{poleId:guid}", Name = nameof(GetPoleById))]
    [Produces("application/json")]
    public ActionResult<PoleDto> GetPoleById([FromRoute] Guid poleId)
    {
        var pole = _repo.FindById(poleId);
        if (pole == null)
            return NotFound();
        var dto = _mapper.Map<PoleDto>(pole);
        return Ok(dto);
    }
    
    [HttpPost]
    [Produces("application/json", "application/xml")]
    public ActionResult<Guid> CreatePole([FromBody] CreatePoleDto? poleDto)
    {
        if (poleDto == null)
            return BadRequest();
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        var poleEntity = _mapper.Map<PoleEntity>(poleDto);
        var insertedPole = _repo.Insert(poleEntity);
        return CreatedAtRoute(nameof(GetPoleById), new{poleId=insertedPole.Id}, insertedPole.Id);
    }
    
    [HttpGet]
    [Produces("application/json")]
    public ActionResult<IEnumerable<PoleDto>> GetPoles()
    {
        var poles = _repo.GetAll();
        return Ok(_mapper.Map<IEnumerable<PoleDto>>(poles));
    }
    
    [HttpDelete("{poleId}")]
    public ActionResult DeletePole([FromRoute] Guid poleId)
    {
        if (_repo.FindById(poleId) == null)
            return NotFound();
        _repo.Delete(poleId);
        return NoContent();
    }
    
    [HttpPost("{pollId}/vote")]
    [Produces("application/json")]
    public ActionResult Vote([FromRoute] Guid pollId, [FromBody] List<Guid> optionsIds)
    {
        var pole = _repo.FindById(pollId);
        if (pole == null)
            return NotFound();
        _repo.UpdateVotes(pollId, optionsIds);
        return NoContent();
    }

    [HttpGet("{pollId}/results")]
    [Produces("application/json")]
    public ActionResult<PoleResultsDto> ShowResults([FromRoute] Guid pollId)
    {
        var pole = _repo.FindById(pollId);
        if (pole == null)
            return NotFound();
        var resultsDto = _mapper.Map<PoleResultsDto>(pole);
        return Ok(resultsDto);
    }
}
