namespace LinqTricks.Models.DTOs;

public class OrderGroupResult
{
    public string Category { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
    public decimal AverageAmount { get; set; }
}