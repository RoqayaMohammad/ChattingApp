using ChattingApp.Models;

namespace ChattingApp.Interfaces
{
    public interface IUSerRepositorye
    {
        void Update();

         Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUserByIdAsync(int id);

        Task<AppUser> GetUSerByUsernameAsync(string username);
    }
}
