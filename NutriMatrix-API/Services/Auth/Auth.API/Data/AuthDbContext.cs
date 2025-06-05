using Auth.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Auth.API.Data
{
    public class AuthDbContext: IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        { }
        public DbSet<NutrientTracking> NutrientTrackings { get; set; }
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
            var metaAdmin = new IdentityUser()
            {
                Id = "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f",
                UserName = "Admin",
                Email = "coolmailedu@gmail.com",
                NormalizedEmail = "coolmailedu@gmail.com".ToUpper(),
                NormalizedUserName = "Admin".ToUpper()
            };
            metaAdmin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(metaAdmin, "SuperSecurePaswwordqwerty@");
            builder.Entity<IdentityUser>().HasData(metaAdmin);

            var adminRole = new List<IdentityUserRole<string>>()
            {
                new()
                {
                    UserId="3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f",
                    RoleId="9d75c886-0a61-40a1-8740-aaf027b8572f"
                }
            };
            builder.Entity<IdentityUserRole<string>>().HasData(adminRole);

            var nuteintIds = new List<long> { 301, 205, 601, 208, 606, 204, 605, 303, 291, 306, 307, 203, 269, 539, 324, 299, 1001, 1006, 1002, 290, 261, 260, 1003, 1004, 1005, 513, 221, 511, 207, 514, 454, 262, 639, 322, 321, 326, 421, 334, 312, 507, 268, 325, 610, 611, 696, 612, 625, 652, 697, 613, 626, 673, 662, 653, 687, 614, 617, 674, 663, 859, 618, 670, 675, 669, 619, 851, 685, 627, 615, 628, 672, 689, 852, 853, 620, 855, 629, 857, 624, 630, 858, 631, 621, 654, 671, 607, 608, 609, 645, 646, 693, 695, 313, 417, 431, 435, 432, 212, 287, 515, 211, 516, 512, 521, 503, 213, 504, 338, 337, 505, 214, 506, 304, 428, 315, 406, 573, 578, 257, 664, 676, 856, 665, 666, 305, 410, 508, 636, 517, 319, 405, 317, 518, 641, 209, 638, 210, 263, 404, 502, 323, 341, 343, 342, 501, 509, 510, 318, 320, 418, 415, 401, 328, 430, 429, 255, 309, 344, 345, 346, 347 };

            var nutrients = nuteintIds.Select((nutrientId, index) =>
            {
                return new NutrientTracking()
                {
                    Id = 161 + index+1,
                    NutrientId = nutrientId,
                    UserId = "3c8b0d12-13e9-4f42-85a4-5d3ce1e7e34f",
                    TargetAmount = 0,
                    IsActive = false
                };
            }).ToList();

            builder.Entity<NutrientTracking>().HasData(nutrients);
        }
    }
}
