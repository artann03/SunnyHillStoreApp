using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunnyHillStore.Controllers.Base;
using SunnyHillStore.Core.Services.Users;
using SunnyHillStore.Model.Entities;
using System.Security.Claims;
using System.Linq;
using SunnyHillStore.Model.Models.Users;
using SunnyHillStore.Model.Models.Products;

namespace SunnyHillStore.Controllers
{
    [Authorize]
    public class UsersController : BaseController<User>
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = AuthorizationConstants.AdminRole)]
        public override async Task<IActionResult> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = AuthorizationConstants.AdminRole)]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserProfileAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userProfileDto = new UserProfileResponseDto
            {
                PublicId = user.PublicId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                LastLoginTime = user.LastLoginTime
            };

            return Ok(userProfileDto);
        }

        [HttpDelete]
        [Authorize(Roles = AuthorizationConstants.AdminRole)]
        public override async Task<IActionResult> DeleteAsync(string id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return Unauthorized();
            }

            try
            {
                var updatedUser = await _userService.UpdateProfileAsync(id, model);
                return Ok(updatedUser);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdateProfileModel model)
        {
            if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest(new { message = "Current and new password are required" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return Unauthorized();
            }

            try
            {
                await _userService.ChangePasswordAsync(id, model.CurrentPassword, model.NewPassword);
                return Ok(new { message = "Password changed successfully" });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
        }
    }
}
