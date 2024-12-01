using System.Text.Json;

namespace WebHookSender;

public class OrderService
{
    public event EventHandler<OrderEventArgs> OrderPlaced;

    public void PlaceOrder(Order order)
    {
        // Business logic for placing an order
        OrderPlaced?.Invoke(this, new OrderEventArgs(order));
    }
}

public class OrderEventArgs : EventArgs
{
    public Order Order { get; }

    public OrderEventArgs(Order order)
    {
        Order = order;
    }
}

public class Order
{
    public string Id { get; }
    public object CustomerName { get; set; }
    public object Total { get; set; }
}

public class PayloadService
{
    public string CreatePayload(Order order)
    {
        var payload = new
        {
            eventType = "order_placed",
            data = new
            {
                orderId = order.Id,
                customer = order.CustomerName,
                total = order.Total
            }
        };

        return JsonSerializer.Serialize(payload);
    }
}

