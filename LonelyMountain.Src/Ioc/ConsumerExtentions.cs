using FluentValidation;
using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Subscriber;
using LonelyMountain.Src.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LonelyMountain.Src.Ioc
{
    public static class ConsumerExtention
    {
        /// <summary>
        /// Inject a new Subscriber with a consumer
        /// </summary>
        /// <param name="service">IService</param>
        /// <param name="config">Iconfiguration</param>
        /// <typeparam name="TMessage">Message component</typeparam>
        /// <typeparam name="TValidator">Validator component</typeparam>
        /// <typeparam name="TConsumer">Consumer component</typeparam>
        /// <returns>IService collection</returns>
        public static IServiceCollection AddRabbitMQConsumer<TMessage, TValidator, TConsumer>(this IServiceCollection service, IConfiguration config)
            where TConsumer : AbstractConsumer<TMessage>
            where TValidator : AbstractValidator<TMessage> =>
                service
                .AddTransient<IValidator<TMessage>, TValidator>()
                .AddScoped<IConsumer<TMessage>, TConsumer>()
                .AddSingleton<ISubscriber, RabbitMQSubscriber<TMessage, TConsumer>>()
                .AddSingleton(RabbitMQConfiguration.Create(config));
    }
}