using MessageProducerApp;
using Moq;
using RabbitMQ.Client;

namespace MessageQueueTests;

public class MessageProducerTests
{
    [Fact]
    public void SendMessage_ShouldPublishMessage()
    {
        var mockChannel = new Mock<IChannel>();
        var producer = new MessageProducer("localhost", "test-queue");

        producer.SendMessageAsync("Test Message");
        
        var props = new BasicProperties();
        props.ContentType = "text/plain";
        props.DeliveryMode = DeliveryModes.Transient;
        mockChannel.Verify(c => c.BasicPublishAsync(
            It.IsAny<string>(),
            "test-queue",
            false,
            props,
            It.IsAny<byte[]>(), new CancellationToken()), Times.Once);
    }
}
