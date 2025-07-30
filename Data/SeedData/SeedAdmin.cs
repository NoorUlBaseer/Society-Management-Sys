using SocietyMng.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace SocietyMng.Data.SeedData
{
    public static class SeedAdmin
    {
        public static User AdminUser
        {
            get
            {
                var passwordHasher = new PasswordHasher<User>();
                var admin = new User
                {
                    Id = 1,
                    FullName = "Mishal Ali",
                    Email = "admin@society.com",
                    PhoneNumber = "1122334455",
                    Gender = "Female", 
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 01, 01),
                    RoleId = 1, //points to admin 
                    PasswordHash = "$2a$08$1m.DC2ZBSdrDYHzW/QiGJexNx9U7TlAuBaBsav6..pGLkh7zJT4Ym" //bcrypt hash for admin@123
                };
                return admin;
            }
        }
    }
}