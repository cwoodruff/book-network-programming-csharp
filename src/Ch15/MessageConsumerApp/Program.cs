using MessageConsumerApp;

var consumer = new MessageConsumer("localhost", "demo-queue");
consumer.StartListening();

Console.WriteLine("Press [Enter] to exit.");
Console.ReadLine();
consumer.Dispose();
