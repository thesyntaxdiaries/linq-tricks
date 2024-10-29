namespace LinqTricks.Models.DTOs;

public class OrderDto
{
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string FormattedAmount { get; set; } = string.Empty;
    public int DaysAgo { get; set; }
}
