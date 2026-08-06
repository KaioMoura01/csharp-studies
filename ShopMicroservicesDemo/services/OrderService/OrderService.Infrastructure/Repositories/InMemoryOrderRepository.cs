using System.Collections.Concurrent;
using OrderService.Application.Abstractions;
using OrderService.Domain;

namespace OrderService.Infrastructure.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentBag<Order> _orders = new();

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        _orders.Add(order);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Order> all = _orders.ToList();
        return Task.FromResult(all);
    }
}
