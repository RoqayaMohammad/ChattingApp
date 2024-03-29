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
        private readonly IUnitOfWork uow;
       
      
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> presencehub;

        public MessageHub(IUnitOfWork uow, IMapper mapper, IHubContext<PresenceHub> presencehub)
        {
            this.uow = uow;
           
           
            this.mapper = mapper;
            this.presencehub = presencehub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext=Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
           var group= await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("updatedGroup", group);

            var messages = await uow.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            if (uow.HasChanges()) await uow.Complete();
            await Clients.Caller.SendAsync("ReciveMessageThread", messages);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
               throw new HubException("You Cannot Sent Messaged to yourself");
            }

            var sender = await uow.UserRepository.GetUSerByUsernameAsync(username);
            var recipient = await uow.UserRepository.GetUSerByUsernameAsync(createMessageDto.RecipientUsername);

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
            var group = await uow.MessageRepository.GetMessageGroup(groupName);
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

            uow.MessageRepository.AddMessage(Message);

            if (await uow.Complete())
            {
               
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(Message));
            }
           // throw new HubException("faild to send message");
        }

        public override  async Task OnDisconnectedAsync(Exception exception)
        {
           var group= await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGRoup");
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare=string.CompareOrdinal(caller, other)<0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group=await uow.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                uow.MessageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            
            if(await uow.Complete()) return group;

            throw new HubException("Failed to add to group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group=await uow.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection=group.Connections.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);
            uow.MessageRepository.RemoveConnection(connection);
            if (await uow.Complete()) return group;

            throw new HubException("Failed to remove from group");
            
        }
    }
}
