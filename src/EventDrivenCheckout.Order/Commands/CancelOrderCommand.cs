namespace EventDrivenCheckout.Order.Commands;

public record CancelOrderCommand(Guid OrderId);
