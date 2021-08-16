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
        private readonly Queue _queue;
        protected readonly string _queueName;
        protected AbstractSubscriber(IServiceProvider services, ILogger<AbstractSubscriber<TMessage, TConsumer>> logger)
        {
            _logger = logger;
            _services = services;
            _queue = typeof(TConsumer);
            _queueName = _queue;
        }
        protected abstract void ActiveQueueSubscribe();
        protected abstract void TopicSubscribe();

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
            _logger.LogInformation("Start subscribing a {type} called {queue}", _queue.Description, _queue.Name);

            if (_queue is ActiveQueue)
                ActiveQueueSubscribe();
            if (_queue is Topic)
                TopicSubscribe();
        }

    }
}