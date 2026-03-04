namespace EventDrivenCheckout.Order.Commands;

public record ConfirmOrderCommand(Guid OrderId, decimal ShippingPrice);
