using SunnyHillStore.Core.Repositories.Base;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Core.Repositories.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task<User> GetByPasswordResetTokenAsync(string resetToken);
    }
}
