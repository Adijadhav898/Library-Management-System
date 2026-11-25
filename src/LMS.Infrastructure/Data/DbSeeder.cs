using BCrypt.Net;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(LibraryDbContext context)
    {
        await context.Database.MigrateAsync();

        // Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role { Name = "Admin", Description = "System Administrator with full access" },
                new Role { Name = "Librarian", Description = "Library staff with circulation and catalog management access" },
                new Role { Name = "Member", Description = "Library member with borrowing privileges" }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // Seed Admin User
        if (!await context.Users.AnyAsync(u => u.Email == "admin@library.com"))
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var adminUser = new User
            {
                Email = "admin@library.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FirstName = "System",
                LastName = "Administrator",
                PhoneNumber = "+1234567890",
                IsActive = true,
                EmailConfirmed = true,
                RoleId = adminRole.Id
            };
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        // Seed Member Types
        if (!await context.MemberTypes.AnyAsync())
        {
            var memberTypes = new List<MemberType>
            {
                new MemberType
                {
                    Name = "Student",
                    Description = "Student member type",
                    MaxBooksAllowed = 3,
                    LoanDurationDays = 14,
                    MaxRenewalsAllowed = 2,
                    FinePerDay = 1.00m,
                    ReservationExpiryHours = 48
                },
                new MemberType
                {
                    Name = "Faculty",
                    Description = "Faculty member type",
                    MaxBooksAllowed = 10,
                    LoanDurationDays = 30,
                    MaxRenewalsAllowed = 3,
                    FinePerDay = 0.50m,
                    ReservationExpiryHours = 72
                },
                new MemberType
                {
                    Name = "General",
                    Description = "General public member type",
                    MaxBooksAllowed = 2,
                    LoanDurationDays = 14,
                    MaxRenewalsAllowed = 1,
                    FinePerDay = 2.00m,
                    ReservationExpiryHours = 24
                }
            };
            await context.MemberTypes.AddRangeAsync(memberTypes);
            await context.SaveChangesAsync();
        }

        // Seed Default Settings
        if (!await context.Settings.AnyAsync())
        {
            var settings = new List<Setting>
            {
                new Setting { Key = "LibraryName", Value = "Central Library", Description = "Name of the library" },
                new Setting { Key = "LibraryAddress", Value = "123 Library Street, City, State 12345", Description = "Library physical address" },
                new Setting { Key = "LibraryPhone", Value = "+1-555-123-4567", Description = "Library contact phone" },
                new Setting { Key = "LibraryEmail", Value = "library@example.com", Description = "Library contact email" },
                new Setting { Key = "DefaultLoanDays", Value = "14", Description = "Default loan duration in days" },
                new Setting { Key = "MaxRenewals", Value = "2", Description = "Maximum number of renewals allowed" },
                new Setting { Key = "FinePerDay", Value = "1.00", Description = "Default fine per day for overdue books" },
                new Setting { Key = "ReservationExpiryHours", Value = "48", Description = "Hours before reservation expires" },
                new Setting { Key = "MaxBooksPerMember", Value = "5", Description = "Maximum books a member can borrow" },
                new Setting { Key = "EnableEmailNotifications", Value = "true", Description = "Enable email notifications" },
                new Setting { Key = "SmtpHost", Value = "smtp.gmail.com", Description = "SMTP server host" },
                new Setting { Key = "SmtpPort", Value = "587", Description = "SMTP server port" },
                new Setting { Key = "SmtpUsername", Value = "", Description = "SMTP username" },
                new Setting { Key = "SmtpPassword", Value = "", Description = "SMTP password" },
                new Setting { Key = "SmtpFromEmail", Value = "noreply@library.com", Description = "From email for notifications" },
                new Setting { Key = "JwtSecretKey", Value = "your-super-secret-jwt-key-change-in-production-min-32-chars", Description = "JWT signing key" },
                new Setting { Key = "JwtIssuer", Value = "LibraryManagementSystem", Description = "JWT issuer" },
                new Setting { Key = "JwtAudience", Value = "LibraryManagementSystem", Description = "JWT audience" },
                new Setting { Key = "AccessTokenExpiryMinutes", Value = "15", Description = "Access token expiry in minutes" },
                new Setting { Key = "RefreshTokenExpiryDays", Value = "7", Description = "Refresh token expiry in days" }
            };
            await context.Settings.AddRangeAsync(settings);
            await context.SaveChangesAsync();
        }

        // Seed Sample Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Fiction", Description = "Fiction books including novels, short stories" },
                new Category { Name = "Non-Fiction", Description = "Non-fiction books including biographies, history" },
                new Category { Name = "Science & Technology", Description = "Science, technology, engineering books" },
                new Category { Name = "Arts & Humanities", Description = "Arts, literature, philosophy books" },
                new Category { Name = "Computer Science", Description = "Programming, software, IT books" },
                new Category { Name = "Mathematics", Description = "Mathematics and statistics books" },
                new Category { Name = "Business & Economics", Description = "Business, finance, economics books" },
                new Category { Name = "Medical & Health", Description = "Medical, health, wellness books" }
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // Seed Sample Publishers
        if (!await context.Publishers.AnyAsync())
        {
            var publishers = new List<Publisher>
            {
                new Publisher { Name = "Penguin Random House", Address = "1745 Broadway, New York, NY 10019", Phone = "+1-212-782-9000", Email = "info@penguinrandomhouse.com", Website = "https://www.penguinrandomhouse.com" },
                new Publisher { Name = "HarperCollins", Address = "195 Broadway, New York, NY 10007", Phone = "+1-212-207-7000", Email = "info@harpercollins.com", Website = "https://www.harpercollins.com" },
                new Publisher { Name = "Simon & Schuster", Address = "1230 Avenue of the Americas, New York, NY 10020", Phone = "+1-212-698-7000", Email = "info@simonandschuster.com", Website = "https://www.simonandschuster.com" },
                new Publisher { Name = "O'Reilly Media", Address = "1005 Gravenstein Highway North, Sebastopol, CA 95472", Phone = "+1-707-827-7000", Email = "info@oreilly.com", Website = "https://www.oreilly.com" },
                new Publisher { Name = "Addison-Wesley", Address = "75 Arlington Street, Boston, MA 02116", Phone = "+1-617-848-7000", Email = "info@addison-wesley.com", Website = "https://www.addison-wesley.com" }
            };
            await context.Publishers.AddRangeAsync(publishers);
            await context.SaveChangesAsync();
        }

        // Seed Sample Authors
        if (!await context.Authors.AnyAsync())
        {
            var authors = new List<Author>
            {
                new Author { FirstName = "Robert", LastName = "Martin", Biography = "Known as Uncle Bob, software engineering expert", BirthDate = new DateTime(1952, 12, 5) },
                new Author { FirstName = "Martin", LastName = "Fowler", Biography = "Author of Patterns of Enterprise Application Architecture", BirthDate = new DateTime(1963, 1, 18) },
                new Author { FirstName = "Eric", LastName = "Evans", Biography = "Author of Domain-Driven Design", BirthDate = new DateTime(1962, 1, 1) },
                new Author { FirstName = "Andrew", LastName = "Hunt", Biography = "Co-author of The Pragmatic Programmer" },
                new Author { FirstName = "David", LastName = "Thomas", Biography = "Co-author of The Pragmatic Programmer" },
                new Author { FirstName = "Kent", LastName = "Beck", Biography = "Creator of Extreme Programming and Test-Driven Development" },
                new Author { FirstName = "Donald", LastName = "Knuth", Biography = "Author of The Art of Computer Programming" }
            };
            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
        }
    }
}