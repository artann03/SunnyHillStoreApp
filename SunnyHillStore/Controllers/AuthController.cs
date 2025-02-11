using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunnyHillStore.Core.Repositories.Users;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SunnyHillStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public AuthController(
            IAuthService authService,
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _authService = authService;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginModel model)
        {
            try
            {
                var response = await _authService.LoginAsync(model);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterModel model)
        {
            try
            {
                var response = await _authService.RegisterAsync(model);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _authService.RevokeTokenAsync(userId);
            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            try
            {
                await _authService.ForgotPasswordAsync(model.Email);
                return Ok(new { message = "If your email is registered, you will receive password reset instructions." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error processing request" });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                await _authService.ResetPasswordAsync(model);
                return Ok(new { message = "Password has been reset successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest(new { message = "Invalid or expired password reset token" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error resetting password" });
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            try
            {
                var exists = await _authService.VerifyEmailExistsAsync(email);
                return Ok(new { exists });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error verifying email" });
            }
        }
    }
} 