using System.ComponentModel.DataAnnotations;

public class ProductManageModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public string Status { get; set; }

    public string Description { get; set; }
} 