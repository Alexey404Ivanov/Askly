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
}