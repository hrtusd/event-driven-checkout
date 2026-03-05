using EventDrivenCheckout.Contracts.Records;

namespace EventDrivenCheckout.Contracts.Events;

public interface BasketCheckedOut
{
    Guid CorrelationId { get; }
    IReadOnlyList<BasketItem> Items { get; }
    bool TriggerFailure { get; }
    string UserId { get; }
}