using OrderService.Application.Abstractions;

namespace OrderService.Application.Orders;

public sealed class ListOrdersHandler(IOrderRepository orderRepository)
{
    public async Task<IReadOnlyCollection<OrderDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetAllAsync(cancellationToken);
        return orders
            .Select(o => new OrderDto(o.Id, o.UserId, o.Product, o.Quantity, o.Status.ToString(), o.Message, o.CreatedAt))
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }
}
