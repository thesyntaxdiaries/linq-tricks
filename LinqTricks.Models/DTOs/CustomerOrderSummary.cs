namespace LinqTricks.Models.DTOs;

public class CustomerOrderSummary
{
    public string CustomerName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
}