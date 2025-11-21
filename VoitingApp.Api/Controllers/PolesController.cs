using Microsoft.AspNetCore.Mvc;
using VoitingApp.Models;

namespace VoitingApp.Controllers;

public class PolesController: Controller
{
    private readonly HttpClient _client;

    public PolesController(HttpClient client)
    {
        _client = client;
    }

    [HttpGet("/poles")]
    public async Task<IActionResult> Index()
    {
        var poles = await _client.GetFromJsonAsync<List<PoleDto>>(
            "http://localhost:5000/api/poles");

        return View("Index", poles);
    }

    [HttpGet("/poles/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var pole = await _client.GetFromJsonAsync<PoleDto>(
            $"http://localhost:5000/api/poles/{id}");

        return View("Details", pole);
    }
}