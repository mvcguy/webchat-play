using System.Threading.Tasks;

namespace WebChatPlay.BackgroundServices
{
    public class MessageProducer
    {
        public async Task SendMessage(IQueueMessage message)
        {
            await FakeMessageQueue.AddMessage(message);
        }
    }
}
