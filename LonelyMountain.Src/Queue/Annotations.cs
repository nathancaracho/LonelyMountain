namespace LonelyMountain.Src.Queues
{
    public class Queue : System.Attribute
    {
        private readonly string _name;
        public Queue(string name) => _name = name;
        public static implicit operator string(Queue queue) => queue._name;
    }

    public class DlqQueue : Queue
    {
        public DlqQueue(string name) : base(name) { }
    }

    public class ActiveQueue : Queue
    {
        public ActiveQueue(string name) : base(name) { }
    }

    public class Topic : Queue
    {
        public Topic(string name) : base(name) { }
    }
}