using FluentValidation;
using LonelyMountain.Src;
using LonelyMountain.Src.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace LonelyMountain.Example.Worker.Consumer
{
    class Program
    {
        static void Main(string[] args) =>
            Bootstrap.Start(service => service
                                        .AddRabbitMQConsumer<CustomerMessage, CustomerConsumer>()
                                        .AddTransient<IValidator<CustomerMessage>, CustomerValidator>()
                                        , args);
    }
}
