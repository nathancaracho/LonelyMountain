using System;
using System.Collections.Generic;
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


        protected override void ActiveQueueSubscribe(string queueName)
        {

            configureQueue();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, eventArgument) =>
            {
                try
                {

                    _logger.LogInformation("The {consumer} consumer was triggered", queueName);

                    var result = await CreateScopeAndProccessMessage(eventArgument.Body.ToArray());

                    if (result.IsFailure)
                    {
                        _channel.BasicNack(eventArgument.DeliveryTag, false, false);
                        _logger.LogError("An error was occurred when try processing {consumer}. Error: {error}", queueName, result.Error);
                    }

                    _logger.LogInformation("The {consumer} was processed with success", queueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An Error occurred when the consumer {queue} try process the message, Error : {error}", queueName, ex.Message);
                    throw;
                }
            };

            _channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);

            void configureQueue()
            {
                try
                {

                    _logger.LogInformation("Start configure active queue {queue}", queueName);

                    var dlqQueue = $"dlq-{queueName}";

                    var queueExchange = $"{queueName}.exchange";
                    var dlqQueueExchange = $"{dlqQueue}.exchange";


                    _channel.ExchangeDeclare(queueExchange, "direct");
                    _channel.QueueDeclare
                    (
                        queueName, true, false, false,
                        new Dictionary<string, object>
                        {
                        {"x-dead-letter-exchange", dlqQueueExchange},
                        {"x-dead-letter-routing-key", dlqQueue}
                        }
                    );
                    _channel.QueueBind(queueName, queueExchange, string.Empty, null);

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

                    _logger.LogInformation("Finish configure active queue {queue}", queueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred when try configure queue {queueName}, Error: {error}", queueName, ex.Message);
                    throw;
                }
            }
        }

    }
}
// https://medium.com/nerd-for-tech/dead-letter-exchanges-at-rabbitmq-net-core-b6348122460d 

