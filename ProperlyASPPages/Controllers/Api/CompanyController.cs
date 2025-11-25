using Microsoft.AspNetCore.Mvc;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyController(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCompanyOrg(int id)
    {
        var company = await _companyRepository.GetCompanyOrgByIdAsync(id);
        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpGet("{id:int}/users")]
    public async Task<IActionResult> GetCompanyUsers(int id)
    {
        var users = await _companyRepository.GetCompanyUsersAsync(id);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompanyOrg([FromBody] dynamic companyOrgData)
    {
        try
        {
            var id = await _companyRepository.CreateCompanyOrgAsync(companyOrgData);
            return CreatedAtAction(nameof(GetCompanyOrg), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
