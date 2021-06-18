using System;

namespace WebChatPlay.MessageInbox
{
    public class InboxMessageDeliveryReport
    {
        public Guid MessageId { get; set; }

        public bool Delivered { get; set; }

        public bool Seen { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime? SeenAt { get; set; }

        public bool IsDeliveryReport => true;
    }

}