public class AuthResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; }
    public string UserRole { get; set; }
} 