using System.Threading.Tasks;

namespace LonelyMountain.Src.Queues
{
    public interface IAcknowledgeManager
    {
        public Task BasicAck();
        public Task BasicNack();
    }
}