using System;
using System.Text.RegularExpressions;

namespace LonelyMountain.Src.Connections
{
    public abstract class AbstractConnection
    {
        private readonly string _connection;
        public AbstractConnection(string connection)
        {

            if (ValidateConnection(connection) is false)
                throw new ArgumentException("The connection not in correct pattern https://www.rabbitmq.com/dotnet-api-guide.html");
            _connection = connection;
        }

        protected abstract bool ValidateConnection(string connection);
        public static implicit operator string(AbstractConnection connection) => connection._connection;
    }


    public class RabbitMQConnection : AbstractConnection
    {
        public RabbitMQConnection(string connection) : base(connection) { }
        protected override bool ValidateConnection(string connection) =>
            new Regex(@"^amqp\:\/\/\w+\:\w+@\w+\:\d+").IsMatch(connection);
    }
}
