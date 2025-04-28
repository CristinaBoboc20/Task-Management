using Microsoft.AspNetCore.Identity;
using TaskManagement.Enums;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public static class SeedData
    {
        public static void SeedUsers(ApplicationDbContext context)
        {
            // Check if there are any users in the db
            // If there are, no need to seed users
            if (context.Users.Any())
            {
                return; 
            }

            // Get admin and user credentials from env variables
            string adminUserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME");
            string userUserName = Environment.GetEnvironmentVariable("USER_USERNAME");
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
            string userPassword = Environment.GetEnvironmentVariable("USER_PASSWORD");

            // Create an Instance of PasswordHasher to hash the passwords
            var hasher = new PasswordHasher<User>();

            // Create new users in db (admin and user) and hash their passwords
            User admin = new User
            {
                UserId = Guid.NewGuid(),
                UserName = adminUserName,
                Password = hasher.HashPassword(null, adminPassword),
                Role = Role.Admin
            };

            User user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = userUserName,
                Password = hasher.HashPassword(null, userPassword),
                Role = Role.User
            };

            // Add these users to the db
            context.Users.AddRange(admin, user);
            
            // Save changes to the db
            context.SaveChanges();
            
            
        }
    }
}
