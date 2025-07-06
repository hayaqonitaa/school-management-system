using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Modules.Admins.Entities;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAdminUser(DatabaseConfig context)
        {
            // Check if admin already exists
            var existingAdmin = await context.Admins.FirstOrDefaultAsync(a => a.Email == "admin@school.com");
            if (existingAdmin != null) return;

            // Create default admin account
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            
            var admin = new Admin
            {
                Username = "admin",
                FullName = "System Administrator",
                Email = "admin@school.com",
                Password = hashedPassword,
                PhoneNumber = "081234567890",
                IsActive = true
            };

            context.Admins.Add(admin);
            await context.SaveChangesAsync();

            // Create corresponding user record
            var adminUser = new User
            {
                IdUser = admin.Id,
                Role = UserRoles.Admin
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            Console.WriteLine($"Default admin created:");
            Console.WriteLine($"Email: admin@school.com");
            Console.WriteLine($"Password: Admin123!");
            Console.WriteLine($"Admin ID: {admin.Id}");
            Console.WriteLine($"User ID: {adminUser.Id}");
        }
    }
}
