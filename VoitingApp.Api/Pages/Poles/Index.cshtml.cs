using Microsoft.AspNetCore.Mvc.RazorPages;
using VoitingApp.Models;

namespace VoitingApp.Pages.Poles;

public class IndexModel : PageModel
{
    private readonly HttpClient _client;

    public IndexModel(HttpClient client)
    {
        _client = client;
    }

    public List<PoleDto> PolesDto { get; set; }

    public void OnGet()
    {
        PolesDto = _client.GetFromJsonAsync<List<PoleDto>>("http://localhost:5000/api/poles").Result ?? [];
    }
}
