using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LonelyMountain.Src.Subscriber.Connections
{
    public static class ConnectionExtention
    {
        public static IServiceCollection AddRabbitMQConnection(this IServiceCollection service, IConfigurationRoot config) =>
            service
            .AddSingleton(new RabbitMQConnection(config.GetSection("MessageBroker").GetSection("RabbitMQ").Value));
    }
}