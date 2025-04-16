using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Data
{
    public class AuthDbContext: IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Name="Admin",
                    NormalizedName="Admin".ToUpper(),
                    Id="9d75c886-0a61-40a1-8740-aaf027b8572f",
                    ConcurrencyStamp="9d75c886-0a61-40a1-8740-aaf027b8572f",
                },
                new IdentityRole()
                {
                    Name="User",
                    NormalizedName="User".ToUpper(),
                    Id="51d825fc-b536-4568-9679-7e8eac698ebe",
                    ConcurrencyStamp="51d825fc-b536-4568-9679-7e8eac698ebe"
                }
            });
        }
    }
}
