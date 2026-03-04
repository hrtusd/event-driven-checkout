
namespace EventDrivenCheckout.Contracts;

public record BasketItem(string ProductId, string Name, decimal Price, int Quantity);