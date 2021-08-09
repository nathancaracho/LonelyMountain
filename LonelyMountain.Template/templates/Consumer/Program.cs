using FluentValidation;
using LonelyMountain.Src;
using LonelyMountain.Src.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace projectName
{
    class Program
    {
        static void Main(string[] args) =>
            Bootstrap.Start(service => service
                                        .AddRabbitMQConsumer<ExampleMessage, projectNameConsumer>()
                                        .AddTransient<IValidator<ExampleMessage>, ExampleValidator>()
                                        , args);
    }
}
