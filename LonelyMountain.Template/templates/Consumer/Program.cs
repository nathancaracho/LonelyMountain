using System.IO;
using FluentValidation;
using LonelyMountain.Src;
using LonelyMountain.Src.Ioc;
using LonelyMountain.Src.Subscriber.Connections;
using Microsoft.Extensions.DependencyInjection;

namespace projectName
{
    class Program
    {
        static void Main(string[] args) =>
            Bootstrap.Start((service, configuration) => service
                                        .AddRabbitMQConsumer<projectNameMessage, projectNameConsumer>()
                                        .AddTransient<IValidator<projectNameMessage>, projectNameValidator>()
                                        .AddRabbitMQConnection(configuration)
                                        , args);
    }
}
