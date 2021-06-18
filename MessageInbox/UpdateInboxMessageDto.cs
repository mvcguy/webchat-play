using System;

namespace WebChatPlay.MessageInbox
{
    public class UpdateInboxMessageDto : IUpdateInboxMessageDto
    {
        public Guid? MessageId { get; set; }
        public bool Seen { get; set; }
        public bool Delivered { get; set; }
        public int RetryCount { get; set; }
        public DateTime? ReceivedAt { get; set; }
    }



}