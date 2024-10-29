// LinqTricks.Tests/LinqExamplesTests.cs
using Xunit;
using LinqTricks.Models;
using LinqTricks.Examples;
using LinqTricks.Models.DTOs;

namespace LinqTricks.Tests;

public class LinqExamplesTests
{
    private readonly List<Customer> _customers;
    private readonly List<Product> _products;
    private readonly List<Order> _orders;

    public LinqExamplesTests()
    {
        // Initialize test data
        _customers = new List<Customer>
        {
            new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                BirthDate = new DateTime(1990, 1, 1),
                Orders = new List<Order>
                {
                    new Order
                    {
                        Id = 1,
                        CustomerId = 1,
                        Amount = 1500,
                        Status = "Pending",
                        Category = "Electronics",
                        OrderDate = DateTime.Now.AddDays(-5)
                    },
                    new Order
                    {
                        Id = 2,
                        CustomerId = 1,
                        Amount = 200,
                        Status = "Completed",
                        Category = "Books",
                        OrderDate = DateTime.Now.AddDays(-3)
                    }
                }
            },
            new Customer
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                BirthDate = new DateTime(1995, 5, 5),
                Orders = new List<Order>
                {
                    new Order
                    {
                        Id = 3,
                        CustomerId = 2,
                        Amount = 2000,
                        Status = "Pending",
                        Category = "Electronics",
                        OrderDate = DateTime.Now.AddDays(-2)
                    }
                }
            }
        };

        _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1200, InStock = true },
            new Product { Id = 2, Name = "Book", Category = "Books", Price = 20, InStock = true },
            new Product { Id = 3, Name = "Phone", Category = "Electronics", Price = 800, InStock = false },
            new Product { Id = 4, Name = "Tablet", Category = "Electronics", Price = 500, InStock = true }
        };

        _orders = _customers.SelectMany(c => c.Orders).ToList();
    }

    [Fact]
    public void GetAllOrders_ShouldFlattenCustomerOrders()
    {
        var result = LinqExamples.GetAllOrders(_customers);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetCustomersWithPendingOrders_ShouldReturnCorrectCustomers()
    {
        var result = LinqExamples.GetCustomersWithPendingOrders(_customers);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetLargeOrders_ShouldReturnOrdersOver1000()
    {
        var result = LinqExamples.GetLargeOrders(_orders.AsQueryable()).ToList();
        Assert.Equal(2, result.Count);
        Assert.All(result, order => Assert.True(order.Amount > 1000));
    }

    [Fact]
    public void GetOrderSummaryByCategory_ShouldGroupCorrectly()
    {
        var result = LinqExamples.GetOrderSummaryByCategory(_orders);
        Assert.Equal(2, result.Count); // Electronics and Books

        var electronics = result.First(r => r.Category == "Electronics");
        Assert.Equal(3500M, electronics.TotalAmount);
        Assert.Equal(2, electronics.Count);
    }

    [Fact]
    public void GetAvailableProducts_ShouldReturnInStockProducts()
    {
        var result = LinqExamples.GetAvailableProducts(_products);
        Assert.Equal(3, result.Count);
        Assert.All(result, p => Assert.True(p.InStock));
        Assert.All(result, p => Assert.True(p.Price > 0));
    }

    [Fact]
    public void GetUniqueCustomers_ShouldRemoveDuplicates()
    {
        var duplicateCustomers = _customers.Concat(new[] { _customers[0] });
        var result = LinqExamples.GetUniqueCustomers(duplicateCustomers);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetTotalAmount_ShouldHandleNullAndEmptyOrders()
    {
        // Arrange
        var ordersWithNull = new List<Order?> { _orders[0], null, _orders[1] };
        
        // Act
        var result = LinqExamples.GetTotalAmount(ordersWithNull!);

        // Assert
        Assert.Equal(1700M, result); // 1500 + 200
    }

    [Fact]
    public void GetCustomerSummaries_ShouldCalculateCorrectTotals()
    {
        // Act
        var result = LinqExamples.GetCustomerSummaries(_customers, _orders);

        // Assert
        Assert.Equal(2, result.Count);
        var johnSummary = result.First(s => s.CustomerName == "John Doe");
        Assert.Equal(2, johnSummary.TotalOrders);
        Assert.Equal(1700M, johnSummary.TotalSpent);
    }

    [Theory]
    [InlineData(1, 2, 2)] // First page
    [InlineData(2, 2, 2)] // Second page
    [InlineData(3, 2, 0)] // Empty page
    [InlineData(1, 0, 0)] // Invalid page size
    public void GetPage_ShouldReturnCorrectItemsForPage(int pageNumber, int pageSize, int expectedCount)
    {
        // Act
        var result = LinqExamples.GetPage(_products, pageNumber, pageSize);

        // Assert
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void TransformOrders_ShouldFormatCorrectly()
    {
        // Act
        var result = LinqExamples.TransformOrders(_orders);

        // Assert
        Assert.Equal(3, result.Count);
        var firstOrder = result.First();
        Assert.Matches(@"ORD-\d{6}", firstOrder.OrderNumber);
        Assert.Contains("$", firstOrder.FormattedAmount);
        Assert.Equal("PENDING", firstOrder.Status);
        Assert.True(firstOrder.DaysAgo >= 0);
    }

    // Performance comparison tests
    [Fact]
    public void ComparePerformance_ForEachVsLinq()
    {
        // Arrange
        var largeCustomerList = GenerateLargeCustomerList(1000);

        // Act - Traditional foreach
        var startForEach = DateTime.Now;
        var ordersForEach = new List<Order>();
        foreach (var customer in largeCustomerList)
        {
            foreach (var order in customer.Orders)
            {
                ordersForEach.Add(order);
            }
        }
        var forEachTime = (DateTime.Now - startForEach).TotalMilliseconds;

        // Act - LINQ
        var startLinq = DateTime.Now;
        var ordersLinq = LinqExamples.GetAllOrders(largeCustomerList);
        var linqTime = (DateTime.Now - startLinq).TotalMilliseconds;

        // Assert
        Assert.Equal(ordersForEach.Count, ordersLinq.Count);
        Console.WriteLine($"ForEach Time: {forEachTime}ms");
        Console.WriteLine($"LINQ Time: {linqTime}ms");
    }

    private List<Customer> GenerateLargeCustomerList(int count)
    {
        var random = new Random(123);
        var customers = new List<Customer>();

        for (int i = 0; i < count; i++)
        {
            var customer = new Customer
            {
                Id = i,
                FirstName = $"FirstName{i}",
                LastName = $"LastName{i}",
                Email = $"email{i}@example.com",
                BirthDate = DateTime.Now.AddYears(-random.Next(20, 60)),
                Orders = new List<Order>()
            };

            var orderCount = random.Next(1, 5);
            for (int j = 0; j < orderCount; j++)
            {
                customer.Orders.Add(new Order
                {
                    Id = i * 10 + j,
                    CustomerId = i,
                    Amount = random.Next(100, 2000),
                    Status = random.Next(2) == 0 ? "Pending" : "Completed",
                    Category = random.Next(2) == 0 ? "Electronics" : "Books",
                    OrderDate = DateTime.Now.AddDays(-random.Next(1, 30))
                });
            }

            customers.Add(customer);
        }

        return customers;
    }
}