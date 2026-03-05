namespace EventDrivenCheckout.Contracts.Records;

public record BasketItem(string ProductId, string Name, decimal Price, int Quantity);