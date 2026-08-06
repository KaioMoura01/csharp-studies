using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Orders;
using OrderService.Domain;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(CreateOrderHandler createOrderHandler, ListOrdersHandler listOrdersHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await listOrdersHandler.HandleAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(request.UserId, request.Product, request.Quantity);
        var order = await createOrderHandler.HandleAsync(command, cancellationToken);

        return order.Status == nameof(OrderStatus.Rejected)
            ? UnprocessableEntity(order)
            : CreatedAtAction(nameof(GetAll), new { }, order);
    }
}

public sealed record CreateOrderRequest(string UserId, string Product, int Quantity);
