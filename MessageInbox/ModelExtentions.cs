using System.Collections.Generic;
using WebChatPlay.BackgroundServices;
using WebChatPlay.Models;

namespace WebChatPlay.MessageInbox
{
    public static class ModelExtentions
    {
        public static InboxMessageDto ToDto(this InboxMessage model)
        {
            return new InboxMessageDto
            {
                MessageId = model.MessageId,
                Recepient = model.Recepient,
                CreatedAt = model.CreatedAt,
                Message = model.Message,
                Delivered = model.Delivered,
                ReceivedAt = model.ReceivedAt,
                RetryCount = model.RetryCount,
                Seen = model.Seen,
                Sender = model.Sender,

            };
        }

        public static InboxMessageDeliveryReport ToDeliveryReport(this InboxMessage model)
        {
            return new InboxMessageDeliveryReport
            {
                MessageId = model.MessageId.GetValueOrDefault(),
                Delivered = model.Delivered,
                Seen = model.Seen,
                DeliveredAt = model.ReceivedAt,
                SeenAt = model.ReceivedAt, // TODO: Need to isolate this in future
            };
        }

        public static InboxMessage ToModel(this InboxMessageDto model)
        {
            return new InboxMessage
            {
                MessageId = model.MessageId,
                Recepient = model.Recepient,
                CreatedAt = model.CreatedAt,
                Message = model.Message,
                Delivered = model.Delivered,
                ReceivedAt = model.ReceivedAt,
                RetryCount = model.RetryCount,
                Seen = model.Seen,
                Sender = model.Sender,
            };
        }

        public static IUpdateInboxMessage ToModel(this IUpdateInboxMessageDto model)
        {
            return new InboxMessage
            {
                MessageId = model.MessageId,
                Delivered = model.Delivered,
                ReceivedAt = model.ReceivedAt,
                RetryCount = model.RetryCount,
                Seen = model.Seen,
            };
        }

        public static UserOnlineStatusDto ToDto(this UserOnlineStatus model)
        {
            return new UserOnlineStatusDto
            {
                UserName = model.UserName,
                IsOnline = model.IsOnline,
                DoNotDisturb = model.DoNotDisturb,
                LastUpdatedAt = model.LastUpdatedAt,
            };
        }

        public static UserOnlineStatus ToDto(this UserOnlineStatusDto dto)
        {
            return new UserOnlineStatus
            {
                UserName = dto.UserName,
                IsOnline = dto.IsOnline,
                DoNotDisturb = dto.DoNotDisturb,
                LastUpdatedAt = dto.LastUpdatedAt,
            };
        }
    }

}