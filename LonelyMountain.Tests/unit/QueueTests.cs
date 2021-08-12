using FluentAssertions;
using LonelyMountain.Src.Queues;
using Xunit;

namespace LonelyMountain.Tests.unit
{
    #region Queues creation

    [ActiveQueue("some-queue")]
    public class ActiveQueueConsumerTest { }

    [DlqQueue("some-dlq-queue")]
    public class DlqQueueConsumer { }

    [Topic("topic")]
    public class TopicConsumerTest { }

    #endregion


    public class QueueTests
    {
        [Fact]
        public void Consumer_ActiveQueue()
        {
            //Act
            var consumer = new ActiveQueueConsumerTest();
            //Assert
            Queue queue = consumer.GetType();

            queue.
            Should()
            .BeOfType<ActiveQueue>();

            string queueName = queue;

            queueName
            .Should()
            .Be("some-queue");

            queue
            .Description
            .Should()
            .Be("Active queue");

        }
        [Fact]
        public void Consumer_TopicQueue()
        {
            //Act
            var consumer = new TopicConsumerTest();
            //Assert
            Queue queue = consumer.GetType();

            queue.
            Should()
            .BeOfType<Topic>();

            string queueName = queue;

            queueName
            .Should()
            .Be("topic");

            queue
            .Description
            .Should()
            .Be("Topic");

        }

        [Fact]
        public void Consumer_DLQQueue()
        {
            //Act
            var consumer = new DlqQueueConsumer();
            //Assert
            Queue queue = consumer.GetType();

            queue.
            Should()
            .BeOfType<DlqQueue>();

            string queueName = queue;

            queueName
            .Should()
            .Be("some-dlq-queue");

            queue
            .Description
            .Should()
            .Be("DLQ queue");

        }
    }
}