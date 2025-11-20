using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProperlyASPPages.Data
{
    public class ProperlyIdentityDbContext(DbContextOptions<ProperlyIdentityDbContext> options) : IdentityDbContext(options)
    {
    }
}
