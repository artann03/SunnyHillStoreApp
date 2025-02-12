using System.ComponentModel.DataAnnotations;

public class UpdateProfileModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
} 