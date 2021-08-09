using System;
using System.Text.RegularExpressions;

namespace LonelyMountain.Src.Subscriber.Connections
{
    public class RabbitMQConnection
    {

        private readonly string _connection;
        private readonly Regex _connectionRegex = new Regex(@"^amqp\:\/\/\w+\:\w+@\w+\:\d+");
        public RabbitMQConnection(string connection)
        {

            if (_connectionRegex.Match(connection).Success is false)
                throw new ArgumentException("The connection not in correct pattern https://www.rabbitmq.com/dotnet-api-guide.html");
            _connection = connection;
        }

        public static implicit operator string(RabbitMQConnection connection) => connection._connection;
    }
}