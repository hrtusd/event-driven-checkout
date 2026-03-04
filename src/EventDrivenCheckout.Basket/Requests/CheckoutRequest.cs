namespace EventDrivenCheckout.Basket.Requests;

public record CheckoutRequest
{
    public string UserId { get; init; } = string.Empty;
    public bool TriggerFailure { get; init; }
}
