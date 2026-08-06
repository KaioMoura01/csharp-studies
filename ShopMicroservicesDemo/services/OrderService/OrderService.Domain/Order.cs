namespace OrderService.Domain;

public sealed class Order
{
    public required string Id { get; init; }
    public required string UserId { get; init; }
    public required string Product { get; init; }
    public required int Quantity { get; init; }
    public required OrderStatus Status { get; init; }
    public required string Message { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
