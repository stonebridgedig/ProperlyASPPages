using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Authorization;
using ProperlyASPPages.Repositories;

namespace ProperlyASPPages.Pages.Admin.Companies;

[Authorize(Policy = SystemAdminPolicies.CanManageCompanies)]
public class IndexModel : PageModel
{
    private readonly ICompanyRepository _companyRepository;

    public IndexModel(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public List<CompanyOrg> Companies { get; set; } = new();
    public Dictionary<int, int> CompanyUserCounts { get; set; } = new();

    public async Task OnGetAsync()
    {
        Companies = await _companyRepository.GetAllCompaniesAsync();
        
        foreach (var company in Companies)
        {
            if (company.CompanyOrgId.HasValue)
            {
                var users = await _companyRepository.GetCompanyUsersAsync(company.CompanyOrgId.Value);
                CompanyUserCounts[company.CompanyOrgId.Value] = users.Count;
            }
        }
    }
}
