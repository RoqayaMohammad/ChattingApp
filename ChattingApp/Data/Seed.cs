using ChattingApp.Models;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp.Data
{
    public class Seed
    {
        public static async Task SeedUsers(AppDbContext context)

        {

            if (await context.AppUsers.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users)

            {

                //using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();

                //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));

                //user.PasswordSalt = hmac.Key;

                context.AppUsers.Add(user);

            }

            await context.SaveChangesAsync();

        }
    }
}
