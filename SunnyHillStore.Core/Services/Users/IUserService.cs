using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities;
using SunnyHillStore.Model.Models.Auth;

namespace SunnyHillStore.Core.Services.Users
{
    public interface IUserService : IBaseService<User>
    {
        Task<User> UpdateProfileAsync(int userId, UpdateProfileModel model);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<User> GetUserProfileAsync(int userId);
        Task<bool> ChangeUserRoleAsync(int userId, string newRole);
    }
}
