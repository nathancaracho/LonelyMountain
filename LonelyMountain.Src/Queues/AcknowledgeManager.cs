using System.Threading.Tasks;
using RabbitMQ.Client;

namespace LonelyMountain.Src.Queues
{
    public class RabbitMQAcknowledgeManager : IAcknowledgeManager
    {
        private readonly IModel _channel;
        private readonly ulong _deliveryTag;

        public RabbitMQAcknowledgeManager(IModel channel, ulong deliveryTag)
        {
            _channel = channel;
            _deliveryTag = deliveryTag;
        }
        public Task BasicAck()
        {
            _channel.BasicAck(_deliveryTag, false);
            return Task.CompletedTask;
        }

        public Task BasicNack()
        {
            _channel.BasicNack(_deliveryTag, false, false);
            return Task.CompletedTask;
        }
    }
}