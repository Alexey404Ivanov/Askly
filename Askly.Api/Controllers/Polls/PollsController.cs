using Askly.Application.DTOs.Polls;
using Microsoft.AspNetCore.Mvc;

namespace Askly.Api.Controllers.Polls;

public class PollsController: Controller
{
    private readonly HttpClient _client;

    public PollsController(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("PollsApiClient");
    }

    [HttpGet("/polls")]
    public async Task<IActionResult> Index()
    {
        var polls = await _client.GetFromJsonAsync<List<PollDto>>(
            "http://localhost:5000/api/polls");

        return View("Index", polls);
    }

    [HttpGet("/polls/{pollId:guid}")]
    public async Task<IActionResult> Details(Guid pollId)
    {
        var poll = await _client.GetFromJsonAsync<PollDto>(
            $"http://localhost:5000/api/polls/{pollId}");
        
        return View("Details", poll);
    }
}