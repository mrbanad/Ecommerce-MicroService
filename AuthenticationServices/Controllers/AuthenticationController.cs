using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationServices.Model;
using AuthenticationServices.Service;
using CommonClassLibrary.Dto.Authentication.Login;
using CommonClassLibrary.Dto.Authentication.Register;
using CommonClassLibrary.Dto.Authentication.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServices.Controllers;

[ApiController]
[Route("[Controller]/[action]")]
public class AuthenticationController : ControllerBase
{
    /// <summary>
    /// SignInManager
    /// </summary>
    private readonly SignInManager<ApplicationUser> _signInManager;

    /// <summary>
    /// TokenService
    /// </summary>
    private readonly ITokenService _tokenService;

    /// <summary>
    /// UserManager
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// UserService
    /// </summary>
    private readonly UserService _userService;

    public AuthenticationController(ITokenService tokenService, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, UserService userService)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    #region Register And Login

    /// <summary>
    /// Register With Email And Password
    /// </summary>
    /// <param name="registerDto"></param>
    /// <remarks>Register User With Same UserName And Email.</remarks>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = new ApplicationUser { UserName = registerDto.UserName, Email = registerDto.UserName };
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
            return Ok();

        return BadRequest();
    }

    /// <summary>
    /// Register With PhoneNumber
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>PhoneNumber,TokenCode,ExpireTime</returns>
    /// <remarks>Register User With Same UserName And PhoneNumber.</remarks>
    [HttpGet]
    public async Task<IActionResult> Register(string phoneNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return BadRequest();

        var model = new UserSearchDto
        {
            Title = null,
            PhoneNumber = phoneNumber,
            Top = 1,
            Skip = 0
        };

        var findUser = await _userService.GetAsync(model, cancellationToken);

        if (findUser.Any())
            return BadRequest();

        var user = new ApplicationUser
        {
            UserName = phoneNumber,
            PhoneNumber = phoneNumber, Email = phoneNumber + "@email.com", EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, Guid.NewGuid() + "Ps1");

        if (result.Succeeded)
            return Ok(new LoginWithPhoneDto
            {
                PhoneNumber = user.PhoneNumber,
                Code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber),
                ExpireMinutes = 2
            });

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Login With UserName And Password
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns>Token,ExpireDate</returns>
    /// <remarks>Login User With Same UserName And Password.</remarks>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password,
            loginDto.IsPersistent, lockoutOnFailure: true);

        if (!result.Succeeded)
            return BadRequest();

        var user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user is null)
            return BadRequest();

        var claims = await _userManager.GetClaimsAsync(user);

        foreach (var role in await _userManager.GetRolesAsync(user))
            claims.Add(new(ClaimTypes.Role, role));

        claims.Add(new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new(ClaimTypes.Name, user.UserName));

        return Ok(new LoginResponseDto
        {
            Token = _tokenService.BuildToken(claims),
            ExpireDate = DateTime.Now.AddHours(3)
        });
    }

    /// <summary>
    /// Login With PhoneNumber
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>PhoneNumber,TokenCode,ExpireTime</returns>
    /// <remarks>Login User With PhoneNumber.</remarks>
    [HttpGet]
    public async Task<IActionResult> Login(string phoneNumber, CancellationToken cancellationToken)
    {
        var model = new UserSearchDto
        {
            PhoneNumber = phoneNumber,
            Top = 1,
            Skip = 0
        };
        var result = await _userService.GetAsync(model, cancellationToken);
        var user = result.FirstOrDefault();
        if (user?.PhoneNumber == null)
            return BadRequest();

        return Ok(new LoginWithPhoneDto
        {
            PhoneNumber = user.PhoneNumber,
            Code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber),
            ExpireMinutes = 2
        });
    }

    #endregion

    #region Confirm

    /// <summary>
    ///  Generate Token For Confirm Email 
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Token, Email</returns>
    /// <remarks>Get Generated String Token  </remarks>
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return BadRequest();

        var result = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return Ok(new ConfirmEmailDto(result, email));
    }

    /// <summary>
    ///  Confirm Email 
    /// </summary>
    /// <param name="confirmEmailDto"></param>
    /// <remarks>Get Email, String Token And Confirm Email</remarks>
    [HttpPost]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
    {
        var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);

        if (user == null)
            return BadRequest();

        var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    #endregion

    #region Password

    /// <summary>
    ///  Send Token Password Recovery With Email 
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Token, Email</returns>
    /// <remarks>Get Generated String Token For Recovery </remarks>
    [HttpGet]
    public async Task<IActionResult> PasswordRecovery([FromBody] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return BadRequest();

        var result = await _userManager.GeneratePasswordResetTokenAsync(user);
        return Ok(new ConfirmEmailDto(result, email));
    }

    /// <summary>
    ///  Send Token And New Password To Change It
    /// </summary>
    /// <param name="passwordRecoveryDto"></param>
    /// <returns>Token, Email</returns>
    /// <remarks>Get Generated String Token For Recovery </remarks>
    [HttpPost]
    public async Task<IActionResult> PasswordRecovery([FromBody] PasswordRecoveryDto passwordRecoveryDto)
    {
        var user = await _userManager.FindByEmailAsync(passwordRecoveryDto.Email);

        if (user == null)
            return BadRequest();

        var result =
            await _userManager.ResetPasswordAsync(user, passwordRecoveryDto.Token, passwordRecoveryDto.Password);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Change Password With Phone Number  
    /// </summary>
    /// <param name="changePasswordDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response</returns>
    /// <remarks>Change PhoneNumber By PhoneNumber</remarks>
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken)
    {
        var model = new UserSearchDto
        {
            PhoneNumber = changePasswordDto.PhoneNumber,
            Top = 1,
            Skip = 0
        };
        var userResponse = await _userService.GetAsync(model, cancellationToken);
        var user = userResponse.FirstOrDefault();
        if (user == null)
            return BadRequest();

        if (!await _userManager.VerifyChangePhoneNumberTokenAsync(user, changePasswordDto.Code,
                changePasswordDto.PhoneNumber) || (DateTime.Now < user.LockoutEnd && user.LockoutEnabled))
        {
            user.AccessFailedCount += 1;
            await _userManager.AccessFailedAsync(user);

            if (user.AccessFailedCount < 5)
                return BadRequest();

            await _userManager.SetLockoutEnabledAsync(user, true);

            var coefficient = user.AccessFailedCount % 5;

            await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(coefficient * 15));

            await _userService.UpdateAsync(user, cancellationToken);

            return BadRequest(DateTime.Now.AddMinutes(coefficient * 15));
        }

        await _userManager.SetLockoutEnabledAsync(user, false);
        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.RemovePasswordAsync(user);
        var result = await _userManager.AddPasswordAsync(user, changePasswordDto.Password);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    #endregion

    #region UserName

    /// <summary>
    ///  Generate Email Token 
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Token, Email</returns>
    /// <remarks>Get Generated String Token For Change UserName </remarks>
    [HttpGet]
    public async Task<IActionResult> ChangeUserName(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return BadRequest();

        var result = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return Ok(new ConfirmEmailDto(result, email));
    }

    /// <summary>
    ///  Get Token And Change UserName
    /// </summary>
    /// <param name="changeUserNameDto"></param>
    /// <returns>Response</returns>
    /// <remarks>Change password</remarks>
    [HttpPost]
    public async Task<IActionResult> ChangeUserName([FromBody] ChangeUserNameDto changeUserNameDto)
    {
        var user = await _userManager.FindByEmailAsync(changeUserNameDto.Email);

        if (user == null)
            return BadRequest();

        var confirm = await _userManager.ConfirmEmailAsync(user, changeUserNameDto.Code);
        if (!confirm.Succeeded)
            return BadRequest(confirm.Errors);

        var setUser = await _userManager.SetUserNameAsync(user, changeUserNameDto.UserName);
        if (!setUser.Succeeded)
            return BadRequest(setUser.Errors);

        await _userManager.UpdateNormalizedUserNameAsync(user);
        return Ok();
    }

    /// <summary>
    /// Change UserName With Phone Number  
    /// </summary>
    /// <param name="changeUserNameByPhoneDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response</returns>
    /// <remarks>Change UserName By Phone Number</remarks>
    [HttpPut]
    public async Task<IActionResult> ChangeUserName([FromBody] ChangeUserNameByPhoneDto changeUserNameByPhoneDto,
        CancellationToken cancellationToken)
    {
        var model = new UserSearchDto
        {
            PhoneNumber = changeUserNameByPhoneDto.PhoneNumber,
            Top = 1,
            Skip = 0
        };
        var userResponse = await _userService.GetAsync(model, cancellationToken);
        var user = userResponse.FirstOrDefault();
        if (user == null)
            return BadRequest();

        if (!await _userManager.VerifyChangePhoneNumberTokenAsync(user, changeUserNameByPhoneDto.Code,
                changeUserNameByPhoneDto.PhoneNumber) || (DateTime.Now < user.LockoutEnd && user.LockoutEnabled))
        {
            user.AccessFailedCount += 1;
            await _userManager.AccessFailedAsync(user);

            if (user.AccessFailedCount < 5)
                return BadRequest();

            await _userManager.SetLockoutEnabledAsync(user, true);

            var coefficient = user.AccessFailedCount % 5;

            await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(coefficient * 15));

            await _userService.UpdateAsync(user, cancellationToken);

            return BadRequest(DateTime.Now.AddMinutes(coefficient * 15));
        }

        await _userManager.SetLockoutEnabledAsync(user, false);
        await _userManager.SetLockoutEndDateAsync(user, null);

        var setUser = await _userManager.SetUserNameAsync(user, changeUserNameByPhoneDto.UserName);
        if (!setUser.Succeeded)
            return BadRequest(setUser.Errors);

        await _userManager.UpdateNormalizedUserNameAsync(user);

        return Ok();
    }

    #endregion

    #region User

    [Authorize("Administrator")]
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<IActionResult> Get(int top, int skip, CancellationToken cancellationToken)
    {
        var items = await _userService.GetAsync(top, skip, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken)
    {
        var item = await _userService.GetAsync(id, cancellationToken);
        return Ok(item);
    }


    [Authorize("Administrator")]
    [HttpPut]
    public virtual async Task<IActionResult> Create([FromBody] ApplicationUser item,
        CancellationToken cancellationToken)
    {
        await _userService.CreateAsync(item, cancellationToken);
        return Ok();
    }

    [Authorize("Administrator")]
    [HttpPost]
    public virtual async Task<IActionResult> Update([FromBody] ApplicationUser item,
        CancellationToken cancellationToken)
    {
        await _userService.UpdateAsync(item, cancellationToken);
        var result = await _userService.GetAsync(item.Id.ToString(), cancellationToken);
        return Ok(result);
    }

    [Authorize("Administrator")]
    [HttpDelete]
    public virtual async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await _userService.RemoveAsync(id, cancellationToken);
        return Ok();
    }

    #endregion
}