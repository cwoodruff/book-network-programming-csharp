using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;

class Program
{
    static async Task Main(string[] args)
    {
        // Create a channel to connect to the gRPC server
        using var channel = GrpcChannel.ForAddress("https://localhost:7204");

        // Create a client for the TodoService
        var client = new TodoService.TodoServiceClient(channel);

        Console.WriteLine("gRPC Client started. Choose an option:");
        while (true)
        {
            Console.WriteLine("1. Add Todo");
            Console.WriteLine("2. Get Todos");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddTodoAsync(client);
                    break;
                case "2":
                    await GetTodosAsync(client);
                    break;
                case "3":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static async Task AddTodoAsync(TodoService.TodoServiceClient client)
    {
        Console.Write("Enter title: ");
        var title = Console.ReadLine();

        Console.Write("Enter description: ");
        var description = Console.ReadLine();

        var request = new TodoRequest { Title = title, Description = description };

        try
        {
            var response = await client.AddTodoAsync(request);
            Console.WriteLine($"Server Response: {response.Status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task GetTodosAsync(TodoService.TodoServiceClient client)
    {
        try
        {
            using var call = client.GetTodos(new Empty());
            Console.WriteLine("Fetching Todos from the server:");

            await foreach (var todo in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"- {todo.Title}: {todo.Description}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
