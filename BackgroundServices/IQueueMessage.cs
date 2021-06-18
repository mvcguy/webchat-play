using System;
using System.Collections.Generic;

namespace WebChatPlay.BackgroundServices
{
    public interface IQueueMessage
    {
        Guid MessageId { get; set; }

        IDictionary<string, object> Props { get; set; }

        string PayloadMessageType { get; set; }

        string Body { get; set; }

        DateTime? CreatedAt { get; set; }
    }
}
