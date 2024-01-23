using ChattingApp.DTOs;
using ChattingApp.Helpers;
using ChattingApp.Models;

namespace ChattingApp.Interfaces
{
    public interface IMessageRepository
    {

        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread (string  currentUserName, string recipirntUserName);

       

       void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string ConnectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectiodId);
    }
}
