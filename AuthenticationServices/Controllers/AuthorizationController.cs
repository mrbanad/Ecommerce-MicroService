using System.Security.Claims;
using AuthenticationServices.Model;
using AuthenticationServices.Service;
using CommonClassLibrary.Dto.Authentication.Claim;
using CommonClassLibrary.Dto.Authentication.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// ...

namespace AuthenticationServices.Controllers;

[ApiController]
[Route("[Controller]/[action]")]
public class AuthorizationController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    private readonly RoleManager<ApplicationRole> _roleManager;

    private readonly RoleService _roleService;

    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationController(RoleManager<ApplicationRole> roleManager, RoleService roleService,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _roleService = roleService;
        _userManager = userManager;
    }

    #region RoleManager

    /// <summary>
    /// Create Role
    /// </summary>
    /// <param name="roleDto"></param>
    /// <returns>StatusCode:200</returns>
    /// <remarks>This method requires authentication.</remarks>
    [HttpPut]
    // [Authorize("Administrator")]
    public async Task<IActionResult> Create(RoleDto roleDto)
    {
        var result = await _roleManager.CreateAsync(new ApplicationRole
        {
            Name = roleDto.Name,
            NormalizedName = roleDto.Name.ToUpper(),
            ConcurrencyStamp = null,
            Title = roleDto.Title,
            Description = roleDto.Description,
            Delete = false,
            Active = true
        });

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var role = await _roleManager.FindByNameAsync(roleDto.Name);
        if (role == null)
            return BadRequest();

        await _roleManager.UpdateNormalizedRoleNameAsync(role);

        return Ok();
    }

    /// <summary>
    /// Update Role
    /// </summary>
    /// <param name="roleDto"></param>
    /// <returns>Updated Role</returns>
    /// <remarks>This method requires authentication.</remarks>
    // [Authorize("Administrator")]
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] RoleDto roleDto)
    {
        var result = await _roleManager.UpdateAsync(new ApplicationRole
        {
            Name = roleDto.Name,
            NormalizedName = roleDto.Name.ToUpper(),
            ConcurrencyStamp = null,
            Title = roleDto.Title,
            Description = roleDto.Description,
            Delete = false,
            Active = true
        });

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var role = await _roleManager.FindByNameAsync(roleDto.Name);
        if (role == null)
            return BadRequest();

        await _roleManager.UpdateNormalizedRoleNameAsync(role);

        return Ok();
    }

    /// <summary>
    /// Delete Role
    /// </summary>
    /// <param name="name"></param>
    /// <returns>StatusCode:200</returns>
    /// <remarks>This method requires authentication.</remarks>
    // [Authorize("Administrator")]
    [HttpDelete]
    public async Task<IActionResult> Delete(string name)
    {
        var role = await _roleManager.FindByNameAsync(name);

        if (role == null)
            return NoContent();

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    #endregion

    #region Claim

    // [Authorize("Administrator")]
    [HttpPost]
    public async Task<IActionResult> AddClaim(ClaimDto claimDto)
    {
        var role = await _roleManager.FindByNameAsync(claimDto.RoleName);

        if (role == null)
            return NoContent();

        var claim = new Claim("Access", claimDto.ClaimValue);
        var result = await _roleManager.AddClaimAsync(role, claim);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    // [Authorize("Administrator")]
    [HttpPost]
    public async Task<IActionResult> RemoveClaim(ClaimDto claimDto)
    {
        var role = await _roleManager.FindByNameAsync(claimDto.RoleName);

        if (role == null)
            return NoContent();

        var claim = new Claim("Access", claimDto.ClaimValue);
        var result = await _roleManager.RemoveClaimAsync(role, claim);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    #endregion

    #region Role

    [Authorize("Administrator")]
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<IActionResult> Get(int top, int skip, CancellationToken cancellationToken)
    {
        var items = await _roleService.GetAsync(top, skip, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken)
    {
        var item = await _roleService.GetAsync(id, cancellationToken);
        return Ok(item);
    }

    #endregion

    #region User

    [HttpPost]
    public async Task<IActionResult> AddRoleToUser(RoleToUserDto roleToUserDto)
    {
        var user = await _userManager.FindByNameAsync(roleToUserDto.UserName);

        if (user == null)
            return NoContent();

        var role = await _roleManager.FindByNameAsync(roleToUserDto.RoleName);

        if (role == null)
            return NoContent();

        var result = await _userManager.AddToRoleAsync(user, role.Name);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRoleToUser(RoleToUserDto roleToUserDto)
    {
        var user = await _userManager.FindByNameAsync(roleToUserDto.UserName);

        if (user == null)
            return NoContent();

        var role = await _roleManager.FindByNameAsync(roleToUserDto.RoleName);

        if (role == null)
            return NoContent();

        var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> AddClaimToUser(ClaimToUserDto claimToUserDto)
    {
        var user = await _userManager.FindByNameAsync(claimToUserDto.UserName);

        if (user == null)
            return NoContent();

        var result = await _userManager.AddClaimAsync(user, new Claim("Access", claimToUserDto.ClaimValue));

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteClaimToUser(ClaimToUserDto claimToUserDto)
    {
        var user = await _userManager.FindByNameAsync(claimToUserDto.UserName);

        if (user == null)
            return NoContent();

        var result = await _userManager.RemoveClaimAsync(user, new Claim("Access", claimToUserDto.ClaimValue));

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    #endregion
}