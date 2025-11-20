using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Properly.API.Models;

namespace ProperlyASPPages.Data
{
    public class ProperlyIdentityDbContext(DbContextOptions<ProperlyIdentityDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
    }
}
