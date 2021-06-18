using System;

namespace WebChatPlay.MessageInbox
{
    /// <summary>
    /// Data transfer model for inbox message
    /// </summary>
    public class InboxMessageDto : UpdateInboxMessageDto
    {
        public string Sender { get; set; }

        public string Recepient { get; set; }

        public string Message { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public class UserOnlineStatusDto
    {
        public string UserName { get; set; }

        public bool IsOnline { get; set; }

        public bool DoNotDisturb { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public bool IsUserStatusReport => true;
    }

    public class UserTypingStatusDto
    {
        public string TargetUser { get; set; }

        public string SourceUser{ get; set; }

        public bool IsTyping { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public bool IsUserTypingStatusReport => true;
    }
}