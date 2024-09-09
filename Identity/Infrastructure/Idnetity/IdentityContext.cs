using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Idnetity
{
    public class IdentityContext : IdentityDbContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>()
                .HasData(
                new IdentityRole()
                {
                    Id = "38EBD7F0-8474-4529-9357-86C7CAFE2AF8",
                    Name = "Owner",
                    NormalizedName = "OWNER"
                },
                new IdentityRole()
                {
                    Id = "6C1335C5-0E73-4091-89C3-4C92A28DF1CD",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }, new IdentityRole()
                {
                    Id = "F5DBC5FD-863E-45FA-B8E9-C0F58A766AAB",
                    Name = "User",
                    NormalizedName = "USER"
                });

            var hasher = new PasswordHasher<IdentityUser>();

            builder.Entity<IdentityUser>()
                .HasData(
                new IdentityUser()
                {
                    Id = "3DB6B2DF-EADB-41FF-9DE4-434BFFB7C626",
                    Email = "alireza80heydri@gmail.com",
                    NormalizedEmail = "ALIREZA80HEYDRI@GMAIL.COM",
                    EmailConfirmed = true,
                    UserName = "Alyrezah",
                    NormalizedUserName = "ALIREZAH",
                    PasswordHash = hasher.HashPassword(null,"Alirezasaina12#"),
                    PhoneNumber = "09167584020",
                    PhoneNumberConfirmed = true,
                });

            builder.Entity<IdentityUserRole<string>>()
                .HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = "38EBD7F0-8474-4529-9357-86C7CAFE2AF8",
                    UserId = "3DB6B2DF-EADB-41FF-9DE4-434BFFB7C626"
                });

            base.OnModelCreating(builder);
        }
    }
}
