using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SunnyHillStore.Core.ApplicationDbContexts;
using SunnyHillStore.Core.Repositories.Users;
using SunnyHillStore.Model.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly IEmailService _emailService;

    public AuthService(
        IUserRepository userRepository,
        IOptions<JwtSettings> jwtSettings,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
        _emailService = emailService;
    }

    public async Task<AuthResponse> LoginAsync(LoginModel model)
    {
        var user = await _userRepository.GetByEmailAsync(model.Email);
        if (user == null || !VerifyPasswordHash(model.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterModel model)
    {
        if (await _userRepository.GetByEmailAsync(model.Email) != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = HashPassword(model.Password),
            Role = RoleConstants.StandardUser,
            PublicId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "System",
            UpdatedBy = "System",
            RefreshToken = "",
            RefreshTokenExpiryTime = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
            return false;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return true;
        }
        var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _userRepository.UpdateAsync(user);

        var resetLink = $"http://localhost:4200/recover-email?token={resetToken}";
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordModel model)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(model.Token);
        if (user == null || 
            user.PasswordResetToken != model.Token || 
            !user.PasswordResetTokenExpiry.HasValue || 
            user.PasswordResetTokenExpiry.Value <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired password reset token");
        }

        user.PasswordHash = HashPassword(model.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> VerifyEmailExistsAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null;
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays);
        user.LastLoginTime = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            UserName = user.Name,
            UserRole = user.Role
        };
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        byte[] hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    private bool VerifyPasswordHash(string password, string storedHash)
    {
        try
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            byte[] storedHashValue = new byte[32];
            Array.Copy(hashBytes, 16, storedHashValue, 0, 32);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] computedHash = pbkdf2.GetBytes(32);

            return computedHash.SequenceEqual(storedHashValue);
        }
        catch
        {
            return false;
        }
    }
} 