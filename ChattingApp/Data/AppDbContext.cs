using ChattingApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ChattingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<AppUser> AppUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>()
       .ToTable("Photos");
        }
    }
}
