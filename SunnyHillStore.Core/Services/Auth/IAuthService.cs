public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginModel model);
    Task<AuthResponse> RegisterAsync(RegisterModel model);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordModel model);
    Task<bool> VerifyEmailExistsAsync(string email);
} 