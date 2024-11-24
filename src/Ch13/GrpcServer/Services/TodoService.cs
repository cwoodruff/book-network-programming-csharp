using Grpc.Core;

namespace GrpcServer.Services;

public class TodoService : GrpcServer.TodoService.TodoServiceBase
{
    private readonly List<TodoItem> _todoList = new();

    public override Task<TodoReply> AddTodo(TodoRequest request, ServerCallContext context)
    {
        var todoItem = new TodoItem { Title = request.Title, Description = request.Description };
        _todoList.Add(todoItem);

        return Task.FromResult(new TodoReply
        {
            Status = $"Task '{request.Title}' added successfully!"
        });
    }

    public override async Task GetTodos(Empty request, IServerStreamWriter<TodoItem> responseStream, ServerCallContext context)
    {
        foreach (var todo in _todoList)
        {
            await responseStream.WriteAsync(todo);
        }
    }
}