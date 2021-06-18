using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebChatPlay.BackgroundServices
{
    /// <summary>
    /// When a message arrives in the queue, all the subsribers are called, The subscriber then should:
    /// <list type="number">    
    /// <item>Since each message belongs to a user, so each user will have a message inbox</item>
    /// <item>Message will be removed immediately from queue and moved to inbox</item>
    /// <item>From inbox we can resend a message if it was not delivered previsouly</item>
    /// <item>The inbox will maintain a flag called: Received=true/false</item>
    /// <item>The inbox will also maintain flag called: RetryCount</item>
    /// <item>There will be a max retry limit</item>
    /// </list>
    /// </summary>
    public static class FakeMessageQueue
    {
        public static ConcurrentQueue<IQueueMessage> Queue = new ConcurrentQueue<IQueueMessage>();

        public static ConcurrentDictionary<string, Func<IQueueMessage, Task>> Subscriptions =
        new ConcurrentDictionary<string, Func<IQueueMessage, Task>>();


        public static async Task AddMessage(IQueueMessage message)
        {
            Queue.Enqueue(message);
            await OnMessage(message);
        }



        public static async Task OnMessage(IQueueMessage message)
        {
            foreach (var item in Subscriptions)
            {
                await item.Value?.Invoke(message);
            }
        }

        public static void Subscribe(string name, Func<IQueueMessage, Task> listener)
        {
            Subscriptions.AddOrUpdate(name, (key) => { return listener; }, (key, value) => { return listener; });
        }

        public static void UnSubscribe(string name)
        {
            Subscriptions.TryRemove(name, out _);
        }
    }
}
