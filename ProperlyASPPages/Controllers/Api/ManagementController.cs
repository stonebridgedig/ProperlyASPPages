using Microsoft.AspNetCore.Mvc;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ManagementController : ControllerBase
{
    private readonly IManagementRepository _managementRepository;

    public ManagementController(IManagementRepository managementRepository)
    {
        _managementRepository = managementRepository;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetManagementOrg(int id)
    {
        var management = await _managementRepository.GetManagementOrgByIdAsync(id);
        if (management == null)
            return NotFound();

        return Ok(management);
    }

    [HttpGet("{id:int}/users")]
    public async Task<IActionResult> GetManagementUsers(int id)
    {
        var users = await _managementRepository.GetManagementUsersAsync(id);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateManagementOrg([FromBody] dynamic managementOrgData)
    {
        try
        {
            var id = await _managementRepository.CreateManagementOrgAsync(managementOrgData);
            return CreatedAtAction(nameof(GetManagementOrg), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
