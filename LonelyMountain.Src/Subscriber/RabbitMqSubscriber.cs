using System;
using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Queues;
using LonelyMountain.Src.Subscriber.Connections;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LonelyMountain.Src.Subscriber
{
    public class RabbitMQSubscriber<TMessage> : AbstractSubscriber<TMessage>, ISubscriber
    {
        private readonly IModel _channel;
        private readonly ILogger _logger;


        public RabbitMQSubscriber(
            IServiceProvider serviceProvider
            , ILogger<RabbitMQSubscriber<TMessage>> logger
            , RabbitMQConnection connection) : base(serviceProvider)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(connection) };
            var conn = factory.CreateConnection();
            _channel = conn.CreateModel();
            _logger = logger;
        }

        public void Subscribe()
        {
            var queue = GetQueue();
            _logger.LogInformation("Start subscribing {consumer} consumer", queue);

            if (queue is ActiveQueue)
                ActiveQueueSubscribe(queue);
        }
        protected override void ActiveQueueSubscribe(string queueName)
        {

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, eventArgument) =>
            {
                _logger.LogInformation("The {consumer} consumer was triggered", queueName);

                var result = await CreateScopeAndProccessMessage(eventArgument.Body.ToArray());

                if (result.IsFailure)
                {
                    _channel.BasicReject(eventArgument.DeliveryTag, false);
                    _logger.LogError("An error was occurred when try processing {consumer}. Error: {error}", queueName, result.Error);
                }
                _channel.BasicNack(eventArgument.DeliveryTag, false, false);

                _logger.LogInformation("The {consumer} was processed with success", queueName);
            };

            _channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

    }
}
// https://medium.com/nerd-for-tech/dead-letter-exchanges-at-rabbitmq-net-core-b6348122460d 