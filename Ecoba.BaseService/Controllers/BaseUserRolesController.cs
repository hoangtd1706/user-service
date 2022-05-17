using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ecoba.BaseService.Infrastructure;
using Ecoba.BaseService.Domain;
using Ecoba.BaseService.Services.UserService;
using Microsoft.EntityFrameworkCore;

namespace Ecoba.BaseService.Controllers;

public abstract class BaseUserRolesController<TContext> : ControllerBase where TContext : BaseDbContext
{
    protected readonly IUserService _userSer;
    protected readonly IUserRoleRepository<TContext> _userRoleRepo;
    protected readonly TContext _context;
    protected string[] ADMIN_NUMBER = new string[] { "0000", "-1000", "-10001" };

    public BaseUserRolesController(
        IUserService userSer,
        IUserRoleRepository<TContext> userRoleRepo,
        TContext context
        )
    {
        _userSer = userSer;
        _userRoleRepo = userRoleRepo;
        _context = context;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserRoleRecord>>> GetUsers([FromQuery] string? role)
    {
        var userNumber = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userNumber == null)
            return Unauthorized();

        var modUserRole = await _userRoleRepo.GetAsync(userNumber, _userRoleRepo.MOD_ROLE);
        if (modUserRole == null)
            return Forbid();

        var userRoles = await _context
                        .UserRoles
                        .Where(x => x.Role != _userRoleRepo.MOD_ROLE)
                        .Where(x => role == null || x.Role == role).ToListAsync();

        var result = new List<UserRoleRecord>();
        if (userRoles.Count > 0)
        {
            var users = await _userSer.GetAllAsync();
            foreach (var userRole in userRoles)
            {
                var user = users.FirstOrDefault(x => x.EmployeeId == userRole.UserNumber);
                var record = new UserRoleRecord
                {
                    UserNumber = userRole.UserNumber,
                    Role = userRole.Role,
                    FullName = user == null ? userRole.UserNumber : user.DisplayName
                };

                result.Add(record);
            }
        }

        return result;
    }

    [HttpGet("users/check-permission")]
    public virtual async Task<ActionResult<bool>> CheckPermission()
    {
        var userNumber = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userNumber == null)
            return Unauthorized();

        var userRole = await _context.UserRoles.AnyAsync(x => x.UserNumber == userNumber);
        return userRole;
    }


    [HttpGet("users/check-role-permission")]
    public async Task<ActionResult<bool>> CheckRolePermission([FromQuery] string role)
    {
        var userNumber = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userNumber == null)
            return Unauthorized();

        var userRole = await _userRoleRepo.GetAsync(userNumber, role);
        return userRole != null;
    }

    [HttpPost("users/{userNumber}/add-role")]
    public async Task<ActionResult> AddUserRole([FromRoute] string userNumber, [FromQuery] string role, string token)
    {
        var currentUserNumber = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (currentUserNumber == null)
            return Unauthorized();

        var modUserRole = await _userRoleRepo.GetAsync(currentUserNumber, _userRoleRepo.MOD_ROLE);
        if (modUserRole == null)
            return Forbid();

        try
        {
            var result = await _userRoleRepo.AddAsync(userNumber, role);
            return Ok(result);
        }
        catch (BaseServiceExceptions ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE: api/Moderators/5
    [HttpDelete("users/{userNumber}/remove-role")]
    public async Task<ActionResult> RemoveUserRole([FromRoute] string userNumber, [FromQuery] string role)
    {
        var currentUserNumber = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (currentUserNumber == null)
            return Unauthorized();

        var modUserRole = await _userRoleRepo.GetAsync(currentUserNumber, _userRoleRepo.MOD_ROLE);
        if (modUserRole == null)
            return Forbid();

        try
        {
            var result = await _userRoleRepo.RemoveAsync(userNumber, role);
            return Ok(result);
        }
        catch (BaseServiceExceptions ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("mods")]
    public async Task<ActionResult<IEnumerable<UserRoleRecord>>> GetMods()
    {
        var userNumber = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userNumber == null)
            return Unauthorized();

        if (!ADMIN_NUMBER.Contains(userNumber))
            return Forbid();

        var modUserRoles = await _context
                        .UserRoles
                        .Where(x => x.Role != _userRoleRepo.MOD_ROLE)
                        .ToListAsync();

        var result = new List<UserRoleRecord>();
        if (modUserRoles.Count > 0)
        {
            var users = await _userSer.GetAllAsync();
            foreach (var userRole in modUserRoles)
            {
                var user = users.FirstOrDefault(x => x.EmployeeId == userRole.UserNumber);
                var record = new UserRoleRecord
                {
                    UserNumber = userRole.UserNumber,
                    Role = userRole.Role,
                    FullName = user == null ? userRole.UserNumber : user.DisplayName
                };

                result.Add(record);
            }
        }

        return result;
    }

    [HttpPost("mods/{userNumber}")]
    public async Task<ActionResult> AddMod([FromRoute] string userNumber)
    {
        var currentUserNumber = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (currentUserNumber == null)
            return Unauthorized();

        if (!ADMIN_NUMBER.Contains(currentUserNumber))
            return Forbid();

        try
        {
            var result = await _userRoleRepo.AddAsync(userNumber, _userRoleRepo.MOD_ROLE);
            return Ok(result);
        }
        catch (BaseServiceExceptions ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE: api/Moderators/5
    [HttpDelete("remove-user/{userNumber}")]
    public async Task<ActionResult> Remove([FromRoute] string userNumber)
    {
        var currentUserNumber = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (currentUserNumber == null)
            return Unauthorized();

        if (!ADMIN_NUMBER.Contains(currentUserNumber))
            return Forbid();

        try
        {
            var result = await _userRoleRepo.RemoveAsync(userNumber, _userRoleRepo.MOD_ROLE);
            return Ok(result);
        }
        catch (BaseServiceExceptions ex)
        {
            return BadRequest(ex);
        }
    }
}

public record UserRoleRecord
{
    public string UserNumber { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
}
