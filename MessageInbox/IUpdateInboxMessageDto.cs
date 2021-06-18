using System;

namespace WebChatPlay.MessageInbox
{
    public interface IUpdateInboxMessageDto
    {
        Guid? MessageId { get; set; }
        bool Seen { get; set; }
        bool Delivered { get; set; }
        int RetryCount { get; set; }
        DateTime? ReceivedAt { get; set; }
    }

}