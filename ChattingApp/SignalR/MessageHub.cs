﻿using AutoMapper;
using ChattingApp.Data;
using ChattingApp.DTOs;
using ChattingApp.Extensions;
using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChattingApp.SignalR
{
    public class MessageHub: Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> presencehub;

        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper, IHubContext<PresenceHub> presencehub)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.presencehub = presencehub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext=Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
            await Clients.Group(groupName).SendAsync("ReciveMessageThread", messages);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
               throw new HubException("You Cannot Sent Messaged to yourself");
            }

            var sender = await userRepository.GetUSerByUsernameAsync(username);
            var recipient = await userRepository.GetUSerByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) { throw new HubException("Not Found User"); }

            var Message = new Models.Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName=GetGroupName(sender.UserName, recipient.UserName);
            var group = await messageRepository.GetMessageGroup(groupName);
            if(group.Connections.Any(x=>x.Username==recipient.UserName)) 
            {
                Message.DateRead = DateTime.Now;
            }
            else
            {
                var connections=await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null)
                {
                    await presencehub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new {username=sender.UserName,sender.KnownAs}
                        );
                }
            }

            messageRepository.AddMessage(Message);

            if (await messageRepository.SaveAllAsync())
            {
               
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(Message));
            }
           // throw new HubException("faild to send message");
        }

        public override  async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare=string.CompareOrdinal(caller, other)<0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var group=await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            return await messageRepository.SaveAllAsync();
        }

        private async Task RemoveFromMessageGroup()
        {
            var connection=await messageRepository.GetConnection(Context.ConnectionId);
            messageRepository.RemoveConnection(connection);
            await messageRepository.SaveAllAsync();
        }
    }
}