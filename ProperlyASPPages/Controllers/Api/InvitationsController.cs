using Microsoft.AspNetCore.Mvc;
using ProperlyASPPages.Repositories;
using Properly.Models;

namespace ProperlyASPPages.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class InvitationsController : ControllerBase
{
    private readonly ICompanyRepository _companyRepository;

    public InvitationsController(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    [HttpGet("token/{token}")]
    public async Task<IActionResult> GetInvitationByToken(string token)
    {
        var invitation = await _companyRepository.GetInvitationByTokenAsync(token);
        if (invitation == null)
            return NotFound();

        return Ok(invitation);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetInvitation(int id)
    {
        var invitation = await _companyRepository.GetInvitationByIdAsync(id);
        if (invitation == null)
            return NotFound();

        return Ok(invitation);
    }

    [HttpGet("company/{companyId:int}")]
    public async Task<IActionResult> GetPendingInvitations(int companyId)
    {
        var invitations = await _companyRepository.GetPendingInvitationsByCompanyAsync(companyId);
        return Ok(invitations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvitation([FromBody] CompanyInvitation invitation)
    {
        try
        {
            var id = await _companyRepository.CreateInvitationAsync(invitation);
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
            var success = await _companyRepository.UpdateInvitationStatusAsync(
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
