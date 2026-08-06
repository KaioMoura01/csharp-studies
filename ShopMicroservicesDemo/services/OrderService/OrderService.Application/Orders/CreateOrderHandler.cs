using OrderService.Application.Abstractions;
using OrderService.Domain;

namespace OrderService.Application.Orders;

public sealed class CreateOrderHandler(IOrderRepository orderRepository, IUserServiceClient userServiceClient)
{
    public async Task<OrderDto> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var user = await userServiceClient.GetUserAsync(command.UserId, cancellationToken);

        Order order;

        if (user is null)
        {
            order = Reject(command, "Usuário não encontrado no user-service");
        }
        else if (!user.Active)
        {
            order = Reject(command, $"Usuário {user.Name} está inativo");
        }
        else
        {
            order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                UserId = command.UserId,
                Product = command.Product,
                Quantity = command.Quantity,
                Status = OrderStatus.Created,
                Message = $"Pedido de {command.Quantity}x \"{command.Product}\" criado para {user.Name}",
                CreatedAt = DateTimeOffset.UtcNow,
            };
        }

        await orderRepository.AddAsync(order, cancellationToken);
        return ToDto(order);
    }

    private static Order Reject(CreateOrderCommand command, string message) => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserId = command.UserId,
        Product = command.Product,
        Quantity = command.Quantity,
        Status = OrderStatus.Rejected,
        Message = message,
        CreatedAt = DateTimeOffset.UtcNow,
    };

    private static OrderDto ToDto(Order order) => new(
        order.Id, order.UserId, order.Product, order.Quantity,
        order.Status.ToString(), order.Message, order.CreatedAt);
}
