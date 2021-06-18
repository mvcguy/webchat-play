using System;

namespace WebChatPlay.MessageInbox
{
    /// <summary>
    /// data access model for inbox message
    /// </summary>
    public class InboxMessage : IUpdateInboxMessage
    {
        public Guid? MessageId { get; set; }

        public string Sender { get; set; }

        public string Recepient { get; set; }

        public string Message { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public bool Seen { get; set; }

        public bool Delivered { get; set; }

        public int RetryCount { get; set; }

    }

    public class UserOnlineStatus
    {
        public string UserName { get; set; }

        public bool IsOnline { get; set; }
        
        public DateTime? LastUpdatedAt { get; set; }

        public bool DoNotDisturb { get; set; }
    }

}