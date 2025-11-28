using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Properly.Models;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IManagementRepository _managementRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(
        IPropertyRepository propertyRepository,
        IManagementRepository managementRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<PropertiesController> logger)
    {
        _propertyRepository = propertyRepository;
        _managementRepository = managementRepository;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProperties()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var managementUser = await _managementRepository.GetManagementUserByIdentityUserIdAsync(user.Id);
            if (managementUser?.ManagementOrgId == null)
                return NotFound("Management organization not found");

            var properties = await _propertyRepository.GetPropertiesByManagementAsync(managementUser.ManagementOrgId.Value);
            return Ok(properties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching properties");
            return StatusCode(500, "An error occurred while fetching properties");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProperty(int id)
    {
        try
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(id);
            if (property == null)
                return NotFound();

            return Ok(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching property {PropertyId}", id);
            return StatusCode(500, "An error occurred while fetching the property");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProperty([FromBody] Property property)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var managementUser = await _managementRepository.GetManagementUserByIdentityUserIdAsync(user.Id);
            if (managementUser?.ManagementOrgId == null)
                return BadRequest("User is not part of a management organization");

            property.ManagementOrgId = managementUser.ManagementOrgId.Value;
            var propertyId = await _propertyRepository.CreatePropertyAsync(property);

            return CreatedAtAction(nameof(GetProperty), new { id = propertyId }, property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property");
            return StatusCode(500, "An error occurred while creating the property");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProperty(int id, [FromBody] Property property)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            property.PropertyId = id;
            var success = await _propertyRepository.UpdatePropertyAsync(property);

            if (!success)
                return NotFound();

            return Ok(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property {PropertyId}", id);
            return StatusCode(500, "An error occurred while updating the property");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        try
        {
            var success = await _propertyRepository.DeletePropertyAsync(id);

            if (!success)
                return NotFound();

            return Ok(new { message = "Property deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property {PropertyId}", id);
            return StatusCode(500, "An error occurred while deleting the property");
        }
    }
}
