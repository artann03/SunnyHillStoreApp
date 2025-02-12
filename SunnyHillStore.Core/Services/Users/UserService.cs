using SunnyHillStore.Core.Repositories.Users;
using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Core.Services.CurrentUser;
using SunnyHillStore.Model.Entities;
using SunnyHillStore.Model.Models.Auth;
using System.Security.Cryptography;

namespace SunnyHillStore.Core.Services.Users
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(
            IUserRepository userRepository, ICurrentUserHelper currentUserService) : base(userRepository, currentUserService)
        {
            _userRepository = userRepository;
        }

        public async Task<User> UpdateProfileAsync(int userId, UpdateProfileModel model)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ApiException("User not found", 404);

            user.Name = model.Name;

            if (model.CurrentPassword != null)
            {
                if (!VerifyPasswordHash(model.CurrentPassword, user.PasswordHash))
                    throw new ApiException("Current password is incorrect", 400);
                user.PasswordHash = HashPassword(model.NewPassword);
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ApiException("User not found", 404);

            if (!VerifyPasswordHash(currentPassword, user.PasswordHash))
                throw new ApiException("Current password is incorrect", 400);

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<User> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ApiException("User not found", 404);

            return user;
        }

        public async Task<bool> ChangeUserRoleAsync(int userId, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ApiException("User not found", 404);

            user.Role = newRole;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
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
}
