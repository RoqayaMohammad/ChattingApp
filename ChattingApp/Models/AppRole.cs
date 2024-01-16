using Microsoft.AspNetCore.Identity;

namespace ChattingApp.Models
{
    public class AppRole:IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
