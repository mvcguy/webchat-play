using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using WebChatPlay.MessageInbox;
using WebChatPlay.SignalRHubs;

namespace WebChatPlay.BackgroundServices
{
    public class MessageConsumer : BackgroundService
    {
        private readonly IHubContext<MessagingHub> hubContext;
        private readonly InboxMaintService inboxService;

        public MessageConsumer(IHubContext<MessagingHub> hubContext, InboxMaintService inboxService)
        {
            this.hubContext = hubContext;
            this.inboxService = inboxService;
        }
        private async Task OnMessageArrived(IQueueMessage message)
        {
            switch (message.PayloadMessageType)
            {
                case Constants.MessagePayloadTypes.InboxMessageTypeCreate:
                    await inboxService.AddMessage(message.Deserialize<InboxMessageDto>());
                    break;
                case Constants.MessagePayloadTypes.InboxMessageTypeUpdate:
                    await inboxService.UpdateMessage(message.Deserialize<UpdateInboxMessageDto>());
                    break;
                
                 case Constants.MessagePayloadTypes.UserStatusReport:
                    await inboxService.UpdateMessage(message.Deserialize<UserOnlineStatusDto>());
                    break;
                
                case Constants.MessagePayloadTypes.UserIsTypingReport:
                    await inboxService.UpdateMessage(message.Deserialize<UserTypingStatusDto>());
                    break;
                
                default:
                    break;
            }

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            FakeMessageQueue.Subscribe("ALI-SHAHID", OnMessageArrived);

            //
            // wait loop
            //
            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    System.Console.WriteLine("Message consumer is shutting down...");
                    break;
                }
                await Task.Delay(1500);
            }
        }
    }
}
