using FluentValidation;
using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Subscriber;
using LonelyMountain.Src.Subscriber.Connections;
using Microsoft.Extensions.Configuration;
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
        public static IServiceCollection AddRabbitMQConsumer<TMessage, TValidator, TConsumer>(this IServiceCollection service, IConfiguration config)
            where TConsumer : AbstractConsumer<TMessage>
            where TValidator : AbstractValidator<TMessage> =>
                service
                .AddTransient<IValidator<TMessage>, TValidator>()
                .AddScoped<IConsumer<TMessage>, TConsumer>()
                .AddSingleton<ISubscriber, RabbitMQSubscriber<TMessage, TConsumer>>()
                .AddSingleton(new RabbitMQConnection(config.GetSection("MessageBroker").GetSection("RabbitMQ").Value));
    }
}