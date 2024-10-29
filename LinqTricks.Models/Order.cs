namespace LinqTricks.Models;


public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
}
