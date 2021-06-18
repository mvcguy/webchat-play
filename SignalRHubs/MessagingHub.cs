using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebChatPlay.BackgroundServices;
using WebChatPlay.MessageInbox;

namespace WebChatPlay.SignalRHubs
{

    [Authorize]
    public class MessagingHub : Hub
    {
        //
        // IMPORTANT:
        //
        // TODO: Make sure that exception handling is done properly
        //      the client application is notified about the errors in sync or async
        //      log the errors for future diagnostics
        //

        public static ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();
        private readonly ILogger<MessagingHub> logger;
        private readonly MessageSender messageSender;
        private readonly IMessagesRepository repo;

        public MessagingHub(ILogger<MessagingHub> logger, MessageSender messageSender, IMessagesRepository repo)
        {
            this.logger = logger;
            this.messageSender = messageSender;
            this.repo = repo;
        }

        public override async Task OnConnectedAsync()
        {
            var userStatus = CreateUserStatus(true);
            await messageSender.SendMessage(userStatus.Serialize());

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userStatus = CreateUserStatus(false);
            await messageSender.SendMessage(userStatus.Serialize());
            await base.OnDisconnectedAsync(exception);
        }

        // [Authorize("DomainRestricted")]
        public async Task<string> OnUserTypingEvent(UserTypingStatusDto userTypingEvent)
        {

            if (userTypingEvent == null) return "event is null";

            var currentUser = this.Context.User.Identity.Name;

            //
            // ignore if current user is not the source user
            //
            if (userTypingEvent.SourceUser != currentUser) return "user does not match!";

            await messageSender.SendMessage(userTypingEvent.Serialize());

            return "message delivered";
        }

        public async Task<string> SendMessage(InboxMessageDto messageDto)
        {
            if (string.IsNullOrWhiteSpace(messageDto.Recepient))
            {
                LogBadRequest("Error: Recpient is empty");
                return string.Empty;
            }

            if (!UserMatching(messageDto.Sender))
            {
                LogBadRequest("Error: User is invalid");
                return string.Empty;
            }

            if (!await AreFriends(messageDto.Sender, messageDto.Recepient))
            {
                LogBadRequest("Error: Invalid request");
                return string.Empty;
            }


            messageDto.Sender = messageDto.Sender;
            messageDto.MessageId = Guid.NewGuid();
            messageDto.CreatedAt = DateTime.UtcNow;
            messageDto.ReceivedAt = null;
            messageDto.Delivered = false;
            messageDto.Seen = false;
            messageDto.RetryCount = 0;
            await this.messageSender.SendMessage(messageDto.Serialize());
            return messageDto.MessageId.ToString();
        }

        public async Task<string> AckMessage(UpdateInboxMessageDto messageDto)
        {
            var message = await repo.GetMessageById(messageDto.MessageId);
            if (message == null)
            {
                LogNotFound("Message not found");
                return string.Empty;
            }
            if (!UserMatching(message.Recepient))
            {
                LogBadRequest("Error: Invalid user");
                return string.Empty;
            }

            await this.messageSender.SendMessage(messageDto.Serialize());
            return messageDto.MessageId.ToString();

        }

        private void LogNotFound(string msg)
        {
            this.logger.LogInformation(FormatMessage(msg, "NotFound"));
        }

        private void LogBadRequest(string msg)
        {
            this.logger.LogError(FormatMessage(msg, "BadRequest"));
        }

        private string FormatMessage(string msg, string tag)
        {
            return $"Tag: {tag}, Message: {msg}";
        }

        private bool UserMatching(string userName)
        {
            var currentUser = this.Context.User.Identity.Name;
            return StringEqual(currentUser, userName);
        }

        private async Task<bool> AreFriends(string user1, string user2)
        {
            return await repo.ValidFriends(user1, user2);
        }

        private UserOnlineStatusDto CreateUserStatus(bool isOnline)
        {
            var connId = this.Context.ConnectionId;
            var userId = this.Context.User.Identity.Name;

            var userStatus = new UserOnlineStatusDto
            {
                UserName = userId,
                IsOnline = isOnline,
                LastUpdatedAt = DateTime.UtcNow,
            };
            return userStatus;
        }

        private bool StringEqual(string a, string b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
