using Askly.Application.DTOs.Users;
using Microsoft.AspNetCore.Mvc;

namespace Askly.Api.Controllers.Users;

public class UsersController : Controller
{
    private readonly HttpClient _client;

    public UsersController(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("UsersApiClient");
    }
    
    [HttpGet("/register")]
    public IActionResult Register()
    {
        return View("Register");
    }
    
    [HttpGet("/login")]
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpGet("/profile")]
    public async Task<IActionResult> Profile()
    {
        var profile = await _client.GetFromJsonAsync<UserProfileDto>(
            $"http://localhost:5000/api/users/profile");
        
        return View("Profile");
    }
}