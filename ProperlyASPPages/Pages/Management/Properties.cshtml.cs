using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProperlyASPPages.Pages.Management;

[Authorize]
public class PropertiesModel : PageModel
{
    public void OnGet()
    {
    }
}
