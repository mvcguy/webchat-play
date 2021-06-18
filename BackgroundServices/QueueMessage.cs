using System;
using System.Collections.Generic;

namespace WebChatPlay.BackgroundServices
{
    public class QueueMessage : IQueueMessage
    {
        public Guid MessageId { get; set; }
        public IDictionary<string, object> Props { get; set; }
        public string Body { get; set; }
        public DateTime? CreatedAt { get; set; }
        
        public string PayloadMessageType { get; set; }
    }
}
