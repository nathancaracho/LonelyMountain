using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Queue;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LonelyMountain.Src.Subscriber
{
    public class RabbitMQSubscriber<TMessage> : ISubscriber
    {
        private readonly IConsumer<TMessage> _consumer;
        private readonly IModel _channel;
        private readonly ILogger _logger;

        public RabbitMQSubscriber(IConsumer<TMessage> consumer, ILogger<RabbitMQSubscriber<TMessage>> logger)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            var connection = factory.CreateConnection();
            _consumer = consumer;
            _channel = connection.CreateModel();
            _logger = logger;
        }

        public void Subscribe()
        {
            var consumer = _consumer.GetConsumerType();
            _logger.LogInformation("Start subscribing {consumer} consumer", consumer);

            if (consumer is ActiveQueue)
                ActiveQueueSubscribe(consumer);
        }

        private void ActiveQueueSubscribe(string queueName)
        {

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, eventArgument) =>
            {
                _logger.LogInformation("The {consumer} consumer was triggered", queueName);

                var result = await _consumer.ProcessMessage(eventArgument.Body.ToArray());

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