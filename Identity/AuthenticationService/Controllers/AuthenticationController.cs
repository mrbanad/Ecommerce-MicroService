using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationService.Model;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;

[ApiController]
[Route("[action]")]
public class AuthenticationController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenBuilder _tokenBuilder;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationController(ITokenBuilder tokenBuilder, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _tokenBuilder = tokenBuilder;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create(string username, string password, string email)
    {
        var user = new ApplicationUser {UserName = username, Email = email};
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> Login(string userName, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(userName, password, false, lockoutOnFailure: false);

        if (!result.Succeeded)
            return BadRequest("Invalid username or password.");

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        return Ok(new
        {
            token = _tokenBuilder.BuildToken(authClaims),
        });
    }
}