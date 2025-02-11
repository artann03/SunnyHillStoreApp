using System.ComponentModel.DataAnnotations;

public class ResetPasswordModel
{
    [Required]
    public string Token { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; }
} 