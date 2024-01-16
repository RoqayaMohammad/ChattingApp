using AutoMapper;
using ChattingApp.DTOs;
using ChattingApp.Extensions;
using ChattingApp.Helpers;
using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChattingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You Cannot Sent Messaged to yourself");
            }

            var sender = await userRepository.GetUSerByUsernameAsync(username);
            var recipient = await userRepository.GetUSerByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) { return NotFound(); }

            var Message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            messageRepository.AddMessage(Message);

            if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(Message));
            return BadRequest("faild to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUsers([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await messageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.ToltalPages));
            return messages;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await messageRepository.GetMessage(id);
            if (message.SenderUsername != username && message.RecipientUsername != username) { return Unauthorized(); }
            if(message.SenderUsername == username ) message.SenderDeleted= true;
            if(message.RecipientUsername==username ) message.RecipientDeleted= true;
            if (message.SenderDeleted && message.RecipientDeleted) { messageRepository.DeleteMessage(message); }
            if (await messageRepository.SaveAllAsync()) return Ok();
            return BadRequest("Problem deleting the messages");
        }
    }
}
