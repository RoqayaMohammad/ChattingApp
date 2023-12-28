using ChattingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options):base(options) { }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}
