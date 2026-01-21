using Askly.Application.DTOs.Users;
using Askly.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Askly.Api.Controllers.Users;

[ApiController]
[Route("api/users")]
public class UsersApiController: ControllerBase
{
    private readonly IUsersService _service;
    
    public UsersApiController(IUsersService service)
    {
        _service = service;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserDto userDto)
    {
        await _service.Register(userDto.UserName, userDto.Email, userDto.Password);
        return NoContent();
    }
    
    [HttpPost("login")]
    [Produces("application/json")]
    public async Task<ActionResult> Login([FromBody] LoginUserDto userDto)
    {
        var token = await _service.Login(userDto.Email, userDto.Password);
        
        HttpContext.Response.Cookies.Append("jwt-token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        
        return Ok(token);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("jwt-token");
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [Produces("application/json")]
    public async Task<ActionResult<UserProfileDto>> GetUserProfileInfo()
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        var profileDto = await _service.GetUserProfileInfo(userId);
        
        return Ok(profileDto);
    }

    [Authorize]
    [HttpPut("me/info")]
    public async Task<ActionResult> UpdateUserProfileInfo([FromBody] UpdateUserInfoDto updateDto)
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        await _service.UpdateUserInfo(userId, updateDto.Name, updateDto.Email);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpPut("me/password")]
    public async Task<ActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordDto updateDto)
    {
        var userId = Guid.Parse(User.FindFirst("userId")!.Value);
        await _service.UpdateUserPassword(userId, updateDto.CurrentPassword, updateDto.NewPassword);
        
        return NoContent();
    }
}