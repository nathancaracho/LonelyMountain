using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using LonelyMountain.Src.Queues;

namespace LonelyMountain.Src.Consumer
{


    /// <summary>
    /// Just a markation interface, used to find listeners on injection.
    /// </summary>
    public interface IConsumer
    { }

    public interface IConsumer<TMessage>
    {
        Task<Result> ProcessMessage(byte[] rawMessage,IAcknowledgeManager acknowledge);
    }

}