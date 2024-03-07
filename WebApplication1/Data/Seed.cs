using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUsers>>(userData);

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.password = ("P@ssw0rd");
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.password));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}
