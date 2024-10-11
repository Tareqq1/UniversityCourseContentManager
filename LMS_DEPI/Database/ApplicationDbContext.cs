using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
namespace LMS_DEPI.APP.Database
{
    public class ApplicationDbContext : IdentityDbContext<UserIdentity>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserIdentity>().Property(u => u.Initials).HasMaxLength(5);
            builder.HasDefaultSchema("Identity");
        }
    }
}
