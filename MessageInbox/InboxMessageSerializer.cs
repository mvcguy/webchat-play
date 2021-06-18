using System;
using WebChatPlay.BackgroundServices;
using System.Text.Json;

namespace WebChatPlay.MessageInbox
{
    public static class MessageSerializer
    {
        public static TResult Deserialize<TResult>(this IQueueMessage queueMessage)
        {
            return JsonSerializer.Deserialize<TResult>(queueMessage.Body);

        }

        public static IQueueMessage Serialize(this InboxMessageDto dto)
        {
            var msg = new QueueMessage
            {
                CreatedAt = DateTime.UtcNow,
                Body = JsonSerializer.Serialize(dto),
                MessageId = Guid.NewGuid(),
                PayloadMessageType = Constants.MessagePayloadTypes.InboxMessageTypeCreate,
            };

            return msg;
        }

        public static IQueueMessage Serialize(this UpdateInboxMessageDto dto)
        {
            var msg = new QueueMessage
            {
                CreatedAt = DateTime.UtcNow,
                Body = JsonSerializer.Serialize(dto),
                MessageId = Guid.NewGuid(),
                PayloadMessageType = Constants.MessagePayloadTypes.InboxMessageTypeUpdate,
            };

            return msg;
        }

        public static IQueueMessage Serialize(this UserOnlineStatusDto dto)
        {
            var msg = new QueueMessage
            {
                MessageId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                PayloadMessageType = Constants.MessagePayloadTypes.UserStatusReport,
                Body = JsonSerializer.Serialize(dto)
            };

            return msg;
        }

        public static IQueueMessage Serialize(this UserTypingStatusDto dto)
        {
            var msg = new QueueMessage
            {
                MessageId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                PayloadMessageType = Constants.MessagePayloadTypes.UserIsTypingReport,
                Body = JsonSerializer.Serialize(dto)
            };

            return msg;
        }

    }
}