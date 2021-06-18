using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace WebChatPlay.BackgroundServices
{
    public class RandomMessageProducer : BackgroundService
    {
        private readonly MessageProducer messageProducer;

        public RandomMessageProducer(MessageProducer messageProducer)
        {
            this.messageProducer = messageProducer;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RandomMessageGenerator(stoppingToken);
        }

        private async Task RandomMessageGenerator(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    System.Console.WriteLine("Message producer is shutting down ...");
                    break;
                }

                await this.messageProducer.SendMessage(new QueueMessage
                {
                    MessageId = Guid.NewGuid(),
                    Body = DateTime.Now.ToUniversalTime().ToString()
                });

                await Task.Delay(3000);
            }
        }
    }
}
