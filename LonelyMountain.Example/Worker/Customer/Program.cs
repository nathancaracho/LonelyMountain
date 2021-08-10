using System.IO;
using FluentValidation;
using LonelyMountain.Src;
using LonelyMountain.Src.Ioc;
using LonelyMountain.Src.Subscriber.Connections;
using Microsoft.Extensions.DependencyInjection;

namespace LonelyMountain.Example.Worker.Consumer
{
    class Program
    {
        static void Main(string[] args) =>
            Bootstrap.Start((service, configuration) => service
                                        .AddRabbitMQConsumer<CustomerMessage, CustomerValidator, CustomerConsumer>(configuration)
                                        , args);
    }
}
