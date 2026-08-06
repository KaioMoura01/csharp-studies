using OrderService.Domain;

namespace OrderService.Application.Abstractions;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken);
}
