using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Properly.Models;
using ProperlyASPPages.Authorization;
using ProperlyASPPages.Repositories;
using System.ComponentModel.DataAnnotations;

namespace ProperlyASPPages.Pages.Admin.Admins;

[Authorize(Policy = SystemAdminPolicies.CanManageAdmins)]
public class CreateModel : PageModel
{
    private readonly ISystemAdminRepository _adminRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(ISystemAdminRepository adminRepository, UserManager<ApplicationUser> userManager)
    {
        _adminRepository = adminRepository;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SystemAdminRole> Roles { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        public bool IsSuperAdmin { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        public string? Notes { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Roles = await _adminRepository.GetAllRolesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Roles = await _adminRepository.GetAllRolesAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existingAdmin = await _adminRepository.GetByEmailAsync(Input.Email);
            if (existingAdmin != null)
            {
                ModelState.AddModelError(string.Empty, "An admin with this email already exists.");
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentAdmin = await _adminRepository.GetByIdentityUserIdAsync(currentUserId!);

            var admin = new SystemAdmin
            {
                IdentityUserId = user.Id,
                RoleId = Input.RoleId,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Phone = Input.Phone,
                Department = Input.Department,
                Title = Input.Title,
                IsSuperAdmin = Input.IsSuperAdmin,
                HireDate = Input.HireDate,
                Notes = Input.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByAdminId = currentAdmin?.SystemAdminId
            };

            await _adminRepository.CreateAsync(admin);

            await _adminRepository.LogActivityAsync(
                currentAdmin!.SystemAdminId,
                "Create Admin",
                $"Created new admin: {admin.FullName} ({admin.Email})",
                "SystemAdmin",
                admin.SystemAdminId.ToString(),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers["User-Agent"].ToString()
            );

            TempData["SuccessMessage"] = "System administrator created successfully.";
            return RedirectToPage("/Admin/Admins/Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error creating admin: {ex.Message}");
            return Page();
        }
    }
}
