using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Queue;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LonelyMountain.Src.Subscriber
{
    public class RabbitMQSubscriber<TMessage> : ISubscriber
    {
        private readonly IConsumer<TMessage> _consumer;
        private readonly IModel _channel;

        public RabbitMQSubscriber(IConsumer<TMessage> consumer)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            var connection = factory.CreateConnection();
            _consumer = consumer;
            _channel = connection.CreateModel();
        }

        public void Subscribe()
        {
            var consumer = _consumer.GetConsumerType();

            if (consumer is ActiveQueue)
                ActiveQueueSubscribe(consumer);
        }

        private void ActiveQueueSubscribe(string queueName)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, eventArgument) =>
            {
                var result = await _consumer.ProcessMessage(eventArgument.Body.ToArray());
                if (result.IsFailure)
                    _channel.BasicReject(eventArgument.DeliveryTag, false);
                _channel.BasicNack(eventArgument.DeliveryTag, false, false);
            };

            _channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

    }
}
// https://medium.com/nerd-for-tech/dead-letter-exchanges-at-rabbitmq-net-core-b6348122460d 