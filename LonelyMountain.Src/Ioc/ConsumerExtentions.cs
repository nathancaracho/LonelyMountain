using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Subscriber;
using Microsoft.Extensions.DependencyInjection;

namespace LonelyMountain.Src.Ioc
{
    public static class ConsumerExtention
    {
        /// <summary>
        /// Inject a new Subscriber with a consumer
        /// </summary>
        /// <param name="service"></param>
        /// <typeparam name="TMessage">Message Type</typeparam>
        /// <typeparam name="TConsumer">Consumer</typeparam>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddRabbitMQConsumer<TMessage, TConsumer>(this IServiceCollection service)
            where TConsumer : AbstractConsumer<TMessage> =>
                service
                .AddScoped<IConsumer<TMessage>, TConsumer>()
                .AddScoped<ISubscriber, RabbitMQSubscriber<TMessage>>();

    }
}