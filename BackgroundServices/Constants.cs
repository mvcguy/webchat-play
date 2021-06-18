using System.Collections.Generic;

namespace WebChatPlay.BackgroundServices
{
    public static class Constants
    {
        public static class PropNames
        {
            public static string Sender = "Sender";
            public static string Recepient = "Recepient";
        }

        public static class MessagePayloadTypes
        {
            public const string InboxMessageTypeCreate = "queue.chat.inbox.create";
            public const string InboxMessageTypeUpdate = "queue.chat.inbox.update";

            public const string UserStatusReport = "queue.chat.inbox.userstatus";

            public const string UserIsTypingReport = "queue.chat.inbox.usertyping";

            public static List<string> All = new List<string>
            {
                 InboxMessageTypeCreate,
                 InboxMessageTypeUpdate,
                 UserStatusReport,
                 UserIsTypingReport
            };

        }
    }
}
