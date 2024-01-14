﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChattingApp.DTOs;
using ChattingApp.Helpers;
using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
                
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
           _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query= _context.Messages.OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username),
                _ => query.Where(u => u.RecipientUsername != messageParams.Username && u.DateRead == null)
            };
            var message = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(message, messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipirntUserName)
        {
           var messages= await _context.Messages.Include(u=>u.Sender).ThenInclude(p=>p.Photos)
                                                .Include(u=>u.Recipient).ThenInclude(p=>p.Photos)
                                                .Where(
                                                       m=>m.RecipientUsername==currentUserName &&
                                                       m.SenderUsername==recipirntUserName||
                                                       m.RecipientUsername==recipirntUserName &&
                                                       m.SenderUsername==currentUserName
               
                                                       ).OrderBy(m=>m.MessageSent).ToListAsync();

            var unreadMessages=messages.Where(m=>m.DateRead==null && m.RecipientUsername== currentUserName).ToList();

            if(unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync()>0;
        }
    }
}
