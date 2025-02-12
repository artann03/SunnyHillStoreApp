public class PurchaseResponseDto
{
    public string OrderNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public int RemainingQuantity { get; set; }
    public string ProductStatus { get; set; }
} 