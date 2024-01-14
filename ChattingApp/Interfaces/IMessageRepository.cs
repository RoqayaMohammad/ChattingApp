using ChattingApp.DTOs;
using ChattingApp.Helpers;
using ChattingApp.Models;

namespace ChattingApp.Interfaces
{
    public interface IMessageRepository
    {

        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMassage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser();
        Task<IEnumerable<MessageDto>> GetMessageThread (int currentUserId, int recipirntId);

        Task<bool> SaveAllAsync();
    }
}
