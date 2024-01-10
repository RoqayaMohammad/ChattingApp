using ChattingApp.DTOs;
using ChattingApp.Models;

namespace ChattingApp.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

         Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUSerByUsernameAsync(string username);

        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto> GetMemberAsync(string username);
    }
}
