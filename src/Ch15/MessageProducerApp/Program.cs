using MessageProducerApp;

var producer = new MessageProducer("localhost", "demo-queue");

Console.WriteLine("Enter messages to send. Type 'exit' to quit.");
string message;
while ((message = Console.ReadLine())?.ToLower() != "exit")
{
    await producer.SendMessageAsync(message);
}

await producer.DisposeAsync();
Console.WriteLine("Producer application terminated.");