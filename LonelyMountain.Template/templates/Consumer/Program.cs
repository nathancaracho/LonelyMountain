using LonelyMountain.Example.Worker.Consumer;
using LonelyMountain.Src;
using LonelyMountain.Src.Ioc;


Bootstrap
    .Start((service, configuration)
            => service
                .AddRabbitMQConsumer<CustomerMessage, CustomerValidator, CustomerConsumer>(configuration)
            , args);
