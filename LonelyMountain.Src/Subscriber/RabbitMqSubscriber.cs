using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LonelyMountain.Src.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LonelyMountain.Src.Subscriber
{
    public class RabbitMQSubscriber<TMessage, TConsumer> : AbstractSubscriber<TMessage, TConsumer>
    {
        private readonly IModel _channel;
        private readonly ILogger _logger;

        private readonly RabbitMQConfiguration _configuration;
        public RabbitMQSubscriber(
            IServiceProvider serviceProvider
            , ILogger<RabbitMQSubscriber<TMessage, TConsumer>> logger
            , RabbitMQConfiguration configuration) : base(serviceProvider, logger)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(configuration.Connection) };
            var conn = factory.CreateConnection();
            _channel = conn.CreateModel();
            _logger = logger;
            _configuration = configuration;
        }

        private async Task QueueEvent(object model, BasicDeliverEventArgs eventArgument)
        {
            try
            {

                _logger.LogInformation("The {consumer} consumer was triggered", _queueName);

                var result = await CreateScopeAndProccessMessage(eventArgument.Body.ToArray());

                if (result.IsFailure)
                {
                    _channel.BasicNack(eventArgument.DeliveryTag, false, false);
                    _logger.LogError("An error was occurred when try processing {consumer}. Error: {error}", _queueName, result.Error);
                }

                _logger.LogInformation("The {consumer} was processed with success", _queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError("An Error occurred when the consumer {queue} try process the message, Error : {error}", _queueName, ex.Message);
                throw;
            }

        }
        private void configureQueue(string queueType, string routingKey = "")
        {
            try
            {

                _logger.LogInformation("Start configure active queue {queue}", _queueName);

                var dlqQueue = $"dlq-{_queueName}";

                var queueExchange = $"{_queueName}.exchange";
                var dlqQueueExchange = $"{dlqQueue}.exchange";


                _channel.ExchangeDeclare(queueExchange, queueType);
                _channel.QueueDeclare
                (
                    _queueName, true, false, false,
                    new Dictionary<string, object>
                    {
                        {"x-dead-letter-exchange", dlqQueueExchange},
                        {"x-dead-letter-routing-key", dlqQueue}
                    }
                );
                _channel.QueueBind(_queueName, queueExchange, routingKey, null);

                _channel.ExchangeDeclare(dlqQueueExchange, "fanout");
                _channel.QueueDeclare
                (
                    dlqQueue, true, false, false,
                    new Dictionary<string, object> {
                        { "x-dead-letter-exchange", queueExchange },
                        { "x-message-ttl", _configuration.Expiration },
                    }
                );
                _channel.QueueBind(dlqQueue, dlqQueueExchange, string.Empty, null);

                _logger.LogInformation("Finish configure active queue {queue}", _queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred when try configure queue {_queueName}, Error: {error}", _queueName, ex.Message);
                throw;
            }
        }

        protected override void ActiveQueueSubscribe()
        {

            configureQueue("direct");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, eventArgument) => await QueueEvent(model, eventArgument);

            _channel.BasicConsume(queue: _queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        protected override void TopicSubscribe()
        {
            configureQueue("topic", Guid.NewGuid().ToString());
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, eventArgument) => await QueueEvent(model, eventArgument);
            _channel.BasicConsume(queue: _queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }
    }
}
// https://medium.com/nerd-for-tech/dead-letter-exchanges-at-rabbitmq-net-core-b6348122460d 

