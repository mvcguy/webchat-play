using System;
using System.Threading.Tasks;
using WebChatPlay.BackgroundServices;

namespace WebChatPlay.MessageInbox
{
    public class MessageSender
    {
        private readonly MessageProducer messageProducer;

        public MessageSender(MessageProducer messageProducer)
        {
            this.messageProducer = messageProducer;
        }

        public async Task SendMessage(IQueueMessage message)
        {
            await this.messageProducer.SendMessage(message);
        }
    }


}