using Food_Delivery.Common;
using Food_Delivery.Models.Dto;
using Food_Delivery.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Food_Delivery.Controllers;

[ApiController]
[Route("api/account")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    [Route("register")]
    [SwaggerOperation(Summary = "Register new user")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterModel userRegisterDto)
    {
        try
        {
            return Ok(await _usersService.RegisterUser(userRegisterDto));
        }
        catch (Exception e)
        {
            return BadRequest(new StatusResponse { Message = e.Message });
        }
    }

    [HttpPost]
    [Route("login")]
    [SwaggerOperation(Summary = "Log in to the system")]
    public async Task<TokenResponse> Login([FromBody] LoginCredentials credentials)
    {
        return await _usersService.LoginUser(credentials);
    }

    [HttpPost]
    [Route("logout")]
    [SwaggerOperation(Summary = "Log out system user")]
    public async Task Logout()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
        {
            throw new Exception("Token not found");
        }

        await _usersService.Logout(token);
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("profile")]
    [SwaggerOperation(Summary = "Get user profile")]
    public async Task<UserDto> GetUserProfile()
    {
        return await _usersService.GetUserProfile(
            Guid.Parse(User.Identity.Name));
    }

    [HttpPut]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("profile")]
    [SwaggerOperation(Summary = "Edit user Profile")]
    public async Task EditUserProfile([FromBody] UserEditModel userEditModel)
    {
        await _usersService.EditUserProfile(
            Guid.Parse(User.Identity.Name), userEditModel);
    }
}