using ChattingApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ChattingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>()
             .HasOne(p => p.AppUser)
             .WithMany(u => u.Photos)
             .HasForeignKey(p => p.AppUserId);


            modelBuilder.Entity<Message>()
                .HasOne(u=>u.Recipient)
                .WithMany(m=>m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
