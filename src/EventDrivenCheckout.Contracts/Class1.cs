namespace EventDrivenCheckout.Contracts;

public record CheckoutStarted(Guid CorrelationId, string UserId, List<BasketItem> Items);
public record BasketItem(string ProductId, string Name, decimal Price, int Quantity);


public record OrderAccepted(Guid OrderId);


public record OrderConfirmed(Guid OrderId);

public record OrderCancelled(Guid OrderId);


public record ShipmentReserved(Guid OrderId);

public record ShipmentRepriced(Guid OrderId, decimal ShippingPrice);