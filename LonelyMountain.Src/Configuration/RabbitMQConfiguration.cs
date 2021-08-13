using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace LonelyMountain.Src.Configuration
{
    public class RabbitMQConfiguration
    {
        public string Connection { get; set; }
        public long Expiration { get; set; } = 1800000;
        private static readonly Regex _connectionValidation = new Regex(@"^amqp\:\/\/\w+\:\w+@\w+\:\d+");
        public static RabbitMQConfiguration Create(IConfiguration config)
        {
            var configuration = config.GetSection("MessageBroker:RabbitMQ").Get<RabbitMQConfiguration>();

            if (configuration == null)
                throw new ArgumentException("The configuration can't be null");

            if (_connectionValidation.IsMatch(configuration.Connection) is false)
                throw new ArgumentException("The connection not was correct pattern https://www.rabbitmq.com/dotnet-api-guide.html");

            if (configuration.Expiration < 1)
                throw new ArgumentException("The expiration time should be higher than 1");

            return configuration;
        }

    }
}