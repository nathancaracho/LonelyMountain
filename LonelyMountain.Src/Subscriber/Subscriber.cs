using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using System;
using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Queues;
using Microsoft.Extensions.Logging;

namespace LonelyMountain.Src.Subscriber
{
    public abstract class AbstractSubscriber<TMessage> : ISubscriber
    {
        private readonly ILogger<AbstractSubscriber<TMessage>> _logger;
        private readonly IServiceProvider _services;

        protected AbstractSubscriber(IServiceProvider services, ILogger<AbstractSubscriber<TMessage>> logger)
        {
            _logger = logger;
            _services = services;
        }

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




        protected abstract void ActiveQueueSubscribe(string queueName);

        public void Subscribe()
        {
            var queue = GetQueue();
            _logger.LogInformation("Start subscribing {consumer} consumer", (string)queue);

            if (queue is ActiveQueue)
                ActiveQueueSubscribe(queue);
        }

        protected Queue GetQueue()
        {
            using var serviceScope = _services.CreateScope();
            var consumer = serviceScope.ServiceProvider.GetRequiredService<IConsumer<TMessage>>();
            return consumer.GetConsumerType();
        }
    }
}