using System.Reflection;

namespace ChattingApp.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        //public string PhotoUrl { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateOnly DateOfBirth;


        public string? KnownAs { get; set; }
        
        public DateTime Created
        { get; set; } = DateTime.UtcNow;

        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public string? Gender { get; set; }
       
        public string? Introduction { get; set; }
       
       public  string? LookingFor { get; set; }
       
       public   string? Interests { get; set; }
       
       public   string? City { get; set; }
       
       public  string? Country { get; set; }

        public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

    }
}
