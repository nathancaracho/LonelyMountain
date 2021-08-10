using System;
using System.Linq;
using LonelyMountain.Src.Consumer;

namespace LonelyMountain.Src.Queues
{

    public static class QueueExtentions
    {
        /// <summary>
        /// Get type of consumer ActiveQueue,DlqQueue or Topic.
        /// </summary>
        /// <param name="consumer">Annoted consumer</param>
        /// <typeparam name="TMessage">consumer message type</typeparam>
        /// <returns>Queue type</returns>
        public static Queue GetConsumerType<TMessage>(this IConsumer<TMessage> consumer)
        {
            Queue queue;

            var properties = consumer.GetType().GetCustomAttributes(typeof(Queue), true);
            if (properties.Any() is false)
                throw new InvalidOperationException("The consumer hasn't queue annotation; Use ActiveQueue, DlqQueue or Topic.");

            queue = (properties.FirstOrDefault() as Queue);
            if (string.IsNullOrEmpty(queue))
                throw new ArgumentException("The Queue Name can't be null, undefined or empty.");

            return queue;
        }
    }
}