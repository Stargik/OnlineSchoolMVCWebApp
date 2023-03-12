using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineSchoolMVCWebApp.Models;

namespace OnlineSchoolMVCWebApp.Data
{
    public class IdentityOnlineSchoolDbContext : IdentityDbContext<User>
    {
        public IdentityOnlineSchoolDbContext(DbContextOptions<IdentityOnlineSchoolDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
