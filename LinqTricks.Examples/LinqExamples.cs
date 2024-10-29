
using LinqTricks.Models;
using LinqTricks.Models.DTOs;

namespace LinqTricks.Examples;

public class LinqExamples
{
    // 1. Replace complex foreach loops with SelectMany
    public static List<Order> GetAllOrders(IEnumerable<Customer> customers)
    {
        // Before:
        // var allOrders = new List<Order>();
        // foreach (var customer in customers)
        // {
        //     foreach (var order in customer.Orders)
        //     {
        //         allOrders.Add(order);
        //     }
        // }

        // After: Clean LINQ
        return customers.SelectMany(customer => customer.Orders).ToList();
    }

    // 2. Filtering nested collections
    public static List<Customer> GetCustomersWithPendingOrders(IEnumerable<Customer> customers)
    {
        return customers.Where(c => c.Orders.Any(o => o.Status == "Pending")).ToList();
    }

    // 3. Memory-efficient processing with deferred execution
    public static IEnumerable<Order> GetLargeOrders(IQueryable<Order> orders)
    {
        return orders.Where(o => o.Amount > 1000)
                    .OrderByDescending(o => o.Amount);
    }

    // 4. Smart grouping and aggregation
    public static List<OrderGroupResult> GetOrderSummaryByCategory(IEnumerable<Order> orders)
    {
        return orders.GroupBy(o => o.Category)
                    .Select(group => new OrderGroupResult
                    {
                        Category = group.Key,
                        TotalAmount = group.Sum(o => o.Amount),
                        Count = group.Count(),
                        AverageAmount = group.Average(o => o.Amount)
                    })
                    .ToList();
    }

    // 5. Multiple conditions with clean syntax
    public static List<Product> GetAvailableProducts(IEnumerable<Product> products)
    {
        return products.Where(p => p.InStock)
                      .Where(p => p.Price > 0)
                      .Where(p => !string.IsNullOrEmpty(p.Category))
                      .OrderBy(p => p.Price)
                      .ToList();
    }

    // 6. Efficient distinct handling
    public static List<Customer> GetUniqueCustomers(IEnumerable<Customer> customers)
    {
        return customers.DistinctBy(c => c.Email).ToList();
    }

    // 7. Safe navigation and null handling
    public static decimal GetTotalAmount(IEnumerable<Order> orders)
    {
        return orders.Where(o => o != null)
                    .DefaultIfEmpty(new Order { Amount = 0 })
                    .Sum(o => o.Amount);
    }

    // 8. Smart joins and lookups
    public static List<CustomerOrderSummary> GetCustomerSummaries(
        IEnumerable<Customer> customers,
        IEnumerable<Order> orders)
    {
        return customers.GroupJoin(
            orders,
            c => c.Id,
            o => o.CustomerId,
            (customer, customerOrders) => new CustomerOrderSummary
            {
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                TotalOrders = customerOrders.Count(),
                TotalSpent = customerOrders.Sum(o => o.Amount)
            })
            .ToList();
    }

    // 9. Pagination with Skip and Take
    public static List<T> GetPage<T>(IEnumerable<T> items, int pageNumber, int pageSize)
    {
        if (pageSize <= 0) return new List<T>();
        return items.Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
    }

    // 10. Complex transformations with Select
    public static List<OrderDto> TransformOrders(IEnumerable<Order> orders)
    {
        return orders.Select(o => new OrderDto
            {
                OrderNumber = $"ORD-{o.Id:D6}",
                Status = o.Status.ToUpper(),
                FormattedAmount = $"${o.Amount:N2}",
                DaysAgo = (DateTime.Now - o.OrderDate).Days
            })
            .ToList();
    }
}