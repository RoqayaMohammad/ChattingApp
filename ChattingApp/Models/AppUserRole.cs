using Microsoft.AspNetCore.Identity;

namespace ChattingApp.Models
{
    public class AppUserRole:IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }

    }
}
