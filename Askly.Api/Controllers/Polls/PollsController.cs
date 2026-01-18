using Askly.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Askly.Api.Controllers.Polls;

public class PollsController: Controller
{
    private readonly IPollsService _service;

    public PollsController(IPollsService service)
    {
        _service = service;
    }

    [HttpGet("/polls")]
    public async Task<IActionResult> Index()
    {
        var polls = await _service.GetAll();

        return View("Index", polls);
    }

    [Authorize]
    [HttpGet("/polls/{pollId:guid}")]
    public async Task<IActionResult> Details(Guid pollId)
    {
        var poll = await _service.GetById(pollId, Guid.Parse(User.FindFirst("userId")!.Value));
        
        return View("Details", poll);
    }
}