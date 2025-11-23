using Properly.Models;

namespace ProperlyASPPages.Repositories;

public interface ICompanyRepository
{
    Task<int> CreateCompanyOrgAsync(CompanyOrg companyOrg);
    Task<int> CreateCompanyUserAsync(CompanyUser companyUser);
    Task<CompanyOrg?> GetCompanyOrgByIdAsync(int companyOrgId);
    Task<CompanyUser?> GetCompanyUserByIdAsync(int companyUserId);
    Task<CompanyUser?> GetCompanyUserByIdentityUserIdAsync(string identityUserId);
    Task<List<CompanyUser>> GetCompanyUsersByIdentityUserIdAsync(string identityUserId);
    Task<List<CompanyOrg>> GetAllCompaniesAsync();
    Task<List<CompanyUser>> GetCompanyUsersAsync(int companyOrgId);
    Task<List<CompanyOrg>> SearchCompanyOrgsByNameAsync(string searchTerm);
    Task<bool> UpdateCompanyUserLastLoginAsync(int companyUserId, DateTime lastLoginAt);
    
    Task<int> CreateInvitationAsync(CompanyInvitation invitation);
    Task<CompanyInvitation?> GetInvitationByTokenAsync(string token);
    Task<CompanyInvitation?> GetInvitationByIdAsync(int invitationId);
    Task<List<CompanyInvitation>> GetPendingInvitationsByCompanyAsync(int companyOrgId);
    Task<bool> UpdateInvitationStatusAsync(int invitationId, InvitationStatus status, string? acceptedByUserId = null);
    Task<bool> IsUserAdminOfCompanyAsync(string identityUserId, int companyOrgId);
    Task<CompanyInvitation?> GetPendingInvitationByEmailAndCompanyAsync(string email, int companyOrgId);
}
