using EventDrivenCheckout.Contracts.Records;

namespace EventDrivenCheckout.Order.Commands;

public record CreateOrderCommand(Guid OrderId, string UserId, IReadOnlyList<BasketItem> Items);
