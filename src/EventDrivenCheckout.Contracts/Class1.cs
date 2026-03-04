namespace EventDrivenCheckout.Contracts;

public record CheckoutStarted(Guid CorrelationId, string UserId, List<BasketItem> Items);
public record BasketItem(string ProductId, string Name, decimal Price, int Quantity);


public record OrderAccepted(Guid CorrelationId, Guid OrderId);


public record OrderShipped(Guid CorrelationId, Guid OrderId, DateTime ShippedAt);
