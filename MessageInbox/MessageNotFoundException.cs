using System;

namespace WebChatPlay.MessageInbox
{
    public class MessageNotFoundException : Exception
    {
        public MessageNotFoundException(string message) : base(message)
        {

        }

    }
}