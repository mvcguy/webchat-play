using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebChatPlay.SignalRHubs;
using System.Web;

namespace WebChatPlay.MessageInbox
{
    public class InboxMaintService
    {
        private readonly IMessagesRepository repository;
        private readonly IHubContext<MessagingHub> hubContext;

        public InboxMaintService(IMessagesRepository repository, IHubContext<MessagingHub> hubContext)
        {
            this.repository = repository;
            this.hubContext = hubContext;
        }

        public async Task AddMessage(InboxMessageDto dto)
        {
            dto.Sender = HttpUtility.HtmlEncode(dto.Sender);
            dto.Recepient = HttpUtility.HtmlEncode(dto.Recepient);
            dto.Message = HttpUtility.HtmlEncode(dto.Message);

            System.Console.WriteLine($"A new message is arrived: {dto.Message}");
            var model = dto.ToModel();

            repository.AddMessage(model);

            await this.hubContext
                        .Clients
                        .All
                        .SendAsync(dto.Recepient, dto.Sender, dto);

            System.Console.WriteLine("Message is delivered");
        }

        public async Task UpdateMessage(IUpdateInboxMessageDto dto)
        {
            var model = dto.ToModel();
            if (model.Seen == true)
            {
                model.Delivered = true;
                model.ReceivedAt = DateTime.UtcNow;
            }
            var updatedMessage = await repository.UpdateMessage(model);
            //
            // inform the sender about message updates
            //            
            await this.hubContext.Clients.All
            .SendAsync(updatedMessage.Sender, "SystemUser", updatedMessage.ToDeliveryReport());
        }

        public async Task UpdateMessage(UserOnlineStatusDto dto)
        {
            var model = dto.ToDto();
            await repository.UpdateUserStatus(model);

            var friends = await repository.GetFriendsList(dto.UserName, 1);

            foreach (var friend in friends)
            {
                await this.hubContext.Clients.All
                            .SendAsync(friend.UserName, "SystemUser", dto);
            }

        }

        public async Task UpdateMessage(UserTypingStatusDto dto)
        {
            await this.hubContext.Clients.All.SendAsync(dto.TargetUser, "SystemUser", dto);
        }
    }
}