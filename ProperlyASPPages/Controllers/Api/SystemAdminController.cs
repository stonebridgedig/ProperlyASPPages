using Microsoft.AspNetCore.Mvc;
using ProperlyASPPages.Repositories;
using Properly.Models;

namespace ProperlyASPPages.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class SystemAdminController : ControllerBase
{
    private readonly ISystemAdminRepository _systemAdminRepository;

    public SystemAdminController(ISystemAdminRepository systemAdminRepository)
    {
        _systemAdminRepository = systemAdminRepository;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSystemAdmin(int id)
    {
        var admin = await _systemAdminRepository.GetByIdAsync(id);
        if (admin == null)
            return NotFound();

        return Ok(admin);
    }

    [HttpGet("identity/{identityUserId}")]
    public async Task<IActionResult> GetSystemAdminByIdentityUserId(string identityUserId)
    {
        var admin = await _systemAdminRepository.GetByIdentityUserIdAsync(identityUserId);
        if (admin == null)
            return NotFound();

        return Ok(admin);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetSystemAdminByEmail(string email)
    {
        var admin = await _systemAdminRepository.GetByEmailAsync(email);
        if (admin == null)
            return NotFound();

        return Ok(admin);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSystemAdmins([FromQuery] bool includeInactive = false)
    {
        var admins = await _systemAdminRepository.GetAllAsync(includeInactive);
        return Ok(admins);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSystemAdmin([FromBody] SystemAdmin admin)
    {
        try
        {
            var createdAdmin = await _systemAdminRepository.CreateAsync(admin);
            return CreatedAtAction(nameof(GetSystemAdmin), new { id = createdAdmin.SystemAdminId }, createdAdmin);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSystemAdmin(int id, [FromBody] SystemAdmin admin)
    {
        try
        {
            admin.SystemAdminId = id;
            var updatedAdmin = await _systemAdminRepository.UpdateAsync(admin);
            return Ok(updatedAdmin);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSystemAdmin(int id)
    {
        try
        {
            var success = await _systemAdminRepository.DeleteAsync(id);
            if (!success)
                return NotFound();

            return Ok(new { message = "System admin deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{identityUserId}/check")]
    public async Task<IActionResult> IsSystemAdmin(string identityUserId)
    {
        var isAdmin = await _systemAdminRepository.IsSystemAdminAsync(identityUserId);
        return Ok(new { isSystemAdmin = isAdmin });
    }

    [HttpGet("activity-logs/{adminId:int}")]
    public async Task<IActionResult> GetActivityLogs(int adminId, [FromQuery] int pageSize = 50, [FromQuery] int pageNumber = 1)
    {
        var logs = await _systemAdminRepository.GetActivityLogsAsync(adminId, pageSize, pageNumber);
        return Ok(logs);
    }

    [HttpGet("activity-logs/recent")]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int count = 100)
    {
        var logs = await _systemAdminRepository.GetRecentActivityAsync(count);
        return Ok(logs);
    }

    [HttpPost("{adminId:int}/activity-logs")]
    public async Task<IActionResult> LogActivity(int adminId, [FromBody] LogActivityRequest request)
    {
        try
        {
            await _systemAdminRepository.LogActivityAsync(
                adminId,
                request.Activity,
                request.Description,
                request.EntityType,
                request.EntityId,
                request.IpAddress,
                request.UserAgent
            );
            return Ok(new { message = "Activity logged successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class LogActivityRequest
{
    public string Activity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
