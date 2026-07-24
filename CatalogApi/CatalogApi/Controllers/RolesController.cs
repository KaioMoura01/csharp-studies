using CatalogApi.DTOs;
using CatalogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("admin")]
public class RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager):ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager  = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    
    [HttpPost]
    [Route("roles")]
    public async Task<IActionResult> Create(string roleName)
    {

        if (await _roleManager.RoleExistsAsync(roleName))
        {
            StatusCode(StatusCodes.Status409Conflict,
                new Response{Status = "Error", Message = $"The role '{roleName}' is already defined."});
        }
        
        var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

        if (roleResult.Succeeded)
        {
            return StatusCode(StatusCodes.Status201Created,
                new Response{Status = "Success", Message = $"Role {roleName} created"});
        }
        
        return StatusCode(StatusCodes.Status400BadRequest,
            new Response{Status = "Error", Message = $"Issue adding the new {roleName} role"});
    }

    [HttpGet("roles")]
    public async Task<IActionResult> ListAll()
    {
        var roles = await _roleManager.Roles
            .Select(r => new { r.Id, r.Name })
            .ToListAsync();

        return Ok(roles);
    }
    
    [HttpGet("roles/{roleName}")]
    public async Task<IActionResult> Get(string roleName)
    {
        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role is null)
        {
            return NotFound("Role not found");
        }

        return Ok(role);
    }

    [HttpPut]
    [Route("roles")]
    public async Task<IActionResult> Update(string roleName, string newRoleName)
    {
        var role =  await _roleManager.FindByNameAsync(roleName);
        var newRole =  await _roleManager.FindByNameAsync(newRoleName);
        
        if (role is null)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new Response{Status = "Error", Message = "Role not found."});
        }
        if (newRole is not null)
        {
            return StatusCode(StatusCodes.Status409Conflict,
                new Response{Status = "Error", Message = "Role is already defined."});
        }
        
        role.Name = newRoleName;
        
        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new Response{Status = "Error", Message = "There's a error updating the role"});
        }
        
        return StatusCode(StatusCodes.Status200OK,
            new Response{Status = "Success", Message = "The role has been updated"});
    }

    [HttpDelete]
    [Route("roles")]
    public async Task<IActionResult> Delete(string roleName)
    {
        var role =  await _roleManager.FindByNameAsync(roleName);

        if (role is null)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new Response{Status = "Error", Message = "Role not found."});
        }
        
        var result  =  await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new Response{Status = "Error", Message = "Error deleting the role"});
        }
        
        return StatusCode(StatusCodes.Status200OK,
            new Response{Status = "Success", Message = "Role has been deleted"});
    }
    
    [HttpPost]
    [Route("add-role-to-user")]
    public async Task<IActionResult> AddUserToRole(string roleName, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new Response{Status = "Error", Message = "User not found."});
        }
        
        var result =  await _userManager.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new Response{Status = "Error", Message = "Unable to add user to role"});
        }
        
        return StatusCode(StatusCodes.Status200OK,
            new Response{Status = "Success", Message = "Role added to user"});
    }
}