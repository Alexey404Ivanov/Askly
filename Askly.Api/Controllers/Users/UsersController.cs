using Askly.Application.DTOs.Users;
using Askly.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Askly.Api.Controllers.Users;

public class UsersController : Controller
{
    private readonly IUsersService _service;
    public UsersController(IUsersService service)
    {
        _service = service;
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

    [Authorize]
    [HttpGet("/me")]
    public async Task<IActionResult> Profile()
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        var profileDto = await _service.GetUserProfileInfo(userId);
        
        return View("Profile", profileDto);
    }
}