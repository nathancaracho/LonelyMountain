using System;
using System.Linq;

namespace LonelyMountain.Src.Queues
{
    public class Queue : Attribute
    {
        public string Name;
        public string Description { get; }
        public Queue(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public static implicit operator string(Queue queue) => queue.Name;

        public static implicit operator Queue(Type type) => GetQueue(type);

        /// <summary>
        /// Get a queue from a consumer
        /// </summary>
        /// <param name="type">consumer type</param>
        /// <returns>queue</returns>
        public static Queue GetQueue(Type type)
        {
            Queue queue;

            var properties = type.GetCustomAttributes(typeof(Queue), true);
            if (properties.Any() is false)
                throw new InvalidOperationException("The consumer hasn't queue annotation; Use ActiveQueue, DlqQueue or Topic.");

            queue = (properties.FirstOrDefault() as Queue);
            if (string.IsNullOrEmpty(queue))
                throw new ArgumentException("The Queue Name can't be null, undefined or empty.");

            return queue;
        }
    }

    public class DlqQueue : Queue
    {
        public DlqQueue(string name) : base(name, "DLQ queue") { }
    }

    public class ActiveQueue : Queue
    {
        public ActiveQueue(string name) : base(name, "Active queue") { }
    }

    public class Topic : Queue
    {
        public Topic(string name) : base(name, "Topic") { }
    }
}