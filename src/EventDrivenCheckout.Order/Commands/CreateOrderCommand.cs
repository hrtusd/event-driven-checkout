using EventDrivenCheckout.Contracts;

namespace EventDrivenCheckout.Order.Commands;

public record CreateOrderCommand(Guid OrderId, string UserId, List<BasketItem> Items);
