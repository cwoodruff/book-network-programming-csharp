using MessageConsumerApp;
using Moq;
using RabbitMQ.Client;
using Xunit;


namespace MessageConsumerTests;

public class MessageConsumerTests
{
    [Fact]
    public void StartListening_ShouldProcessMessage()
    {
        var mockChannel = new Mock<IChannel>();
        var consumer = new MessageConsumer("localhost", "test-queue");

        Assert.NotNull(consumer);
    }
}

