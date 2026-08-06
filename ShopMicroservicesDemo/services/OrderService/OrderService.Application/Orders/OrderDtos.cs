namespace OrderService.Application.Orders;

public sealed record CreateOrderCommand(string UserId, string Product, int Quantity);

public sealed record OrderDto(string Id, string UserId, string Product, int Quantity, string Status, string Message, DateTimeOffset CreatedAt);
