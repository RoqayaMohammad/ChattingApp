using ChattingApp.DTOs;
using ChattingApp.Helpers;
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

        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username);
    }
}
