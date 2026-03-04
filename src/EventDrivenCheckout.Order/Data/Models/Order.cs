namespace EventDrivenCheckout.Order.Data.Models;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public decimal? ShippingPrice { get; set; }
    public decimal? TotalPrice { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}
