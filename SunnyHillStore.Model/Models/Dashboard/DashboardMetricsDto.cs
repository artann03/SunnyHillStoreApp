public class DashboardMetricsDto
{
    public int TotalProducts { get; set; }
    public int ActiveUsers { get; set; }
    public int OutOfStockProducts { get; set; }
    public int LowStockProducts { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public List<ProductSaleMetric> TopSellingProducts { get; set; }
    public List<MonthlyRevenue> MonthlyRevenues { get; set; }
}

public class ProductSaleMetric
{
    public string ProductName { get; set; }
    public int SalesCount { get; set; }
    public decimal Revenue { get; set; }
}

public class MonthlyRevenue
{
    public string Month { get; set; }
    public decimal Revenue { get; set; }
} 