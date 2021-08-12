using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using System;
using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Queues;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace LonelyMountain.Src.Subscriber
{
    public abstract class AbstractSubscriber<TMessage, TConsumer> : ISubscriber
    {
        private readonly ILogger<AbstractSubscriber<TMessage, TConsumer>> _logger;
        private readonly IServiceProvider _services;

        protected AbstractSubscriber(IServiceProvider services, ILogger<AbstractSubscriber<TMessage, TConsumer>> logger)
        {
            _logger = logger;
            _services = services;
        }
        protected abstract void ActiveQueueSubscribe(string queueName);

        /// <summary>
        /// Create new consumer scope and proccess message
        /// </summary>
        /// <param name="message">message byte array</param>
        /// <returns>Result</returns>
        protected async Task<Result> CreateScopeAndProccessMessage(byte[] message)
        {
            using var serviceScope = _services.CreateScope();
            var consumer = serviceScope.ServiceProvider.GetRequiredService<IConsumer<TMessage>>();
            return await consumer.ProcessMessage(message);
        }

        public void Subscribe()
        {
            Queue queue = typeof(TConsumer);
            _logger.LogInformation("Start subscribing a {type} called {queue}", queue.Description, queue.Name);

            if (queue is ActiveQueue)
                ActiveQueueSubscribe(queue);
        }


    }
}