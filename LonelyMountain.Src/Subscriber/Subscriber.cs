using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using System;
using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Queues;

namespace LonelyMountain.Src.Subscriber
{
    public abstract class AbstractSubscriber<TMessage>
    {
        private readonly IServiceProvider _services;

        protected AbstractSubscriber(IServiceProvider services) => _services = services;

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

        protected Queue GetQueue()
        {
            using var serviceScope = _services.CreateScope();
            var consumer = serviceScope.ServiceProvider.GetRequiredService<IConsumer<TMessage>>();
            return consumer.GetConsumerType();
        }
        protected abstract void ActiveQueueSubscribe(string queueName);
    }
}