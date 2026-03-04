namespace EventDrivenCheckout.Contracts.Events;

public interface BasketCheckedOut
{
    Guid CorrelationId { get; init; }
    List<BasketItem> Items { get; init; }
    bool TriggerFailure { get; init; }
    string UserId { get; init; }
}