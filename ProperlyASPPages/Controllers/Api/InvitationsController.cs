using Microsoft.AspNetCore.Mvc;
using ProperlyASPPages.Repositories;
using Properly.Models;

namespace ProperlyASPPages.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class InvitationsController : ControllerBase
{
    private readonly IManagementRepository _managementRepository;

    public InvitationsController(IManagementRepository managementRepository)
    {
        _managementRepository = managementRepository;
    }

    [HttpGet("token/{token}")]
    public async Task<IActionResult> GetInvitationByToken(string token)
    {
        var invitation = await _managementRepository.GetInvitationByTokenAsync(token);
        if (invitation == null)
            return NotFound();

        return Ok(invitation);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetInvitation(int id)
    {
        var invitation = await _managementRepository.GetInvitationByIdAsync(id);
        if (invitation == null)
            return NotFound();

        return Ok(invitation);
    }

    [HttpGet("management/{managementId:int}")]
    public async Task<IActionResult> GetPendingInvitations(int managementId)
    {
        var invitations = await _managementRepository.GetPendingInvitationsByManagementAsync(managementId);
        return Ok(invitations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvitation([FromBody] ManagementInvitation invitation)
    {
        try
        {
            var id = await _managementRepository.CreateInvitationAsync(invitation);
            return CreatedAtAction(nameof(GetInvitation), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateInvitationStatus(int id, [FromBody] UpdateInvitationStatusRequest request)
    {
        try
        {
            var success = await _managementRepository.UpdateInvitationStatusAsync(
                id,
                request.Status,
                request.AcceptedByUserId
            );

            if (!success)
                return NotFound();

            return Ok(new { message = "Invitation status updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class UpdateInvitationStatusRequest
{
    public InvitationStatus Status { get; set; }
    public string? AcceptedByUserId { get; set; }
}
