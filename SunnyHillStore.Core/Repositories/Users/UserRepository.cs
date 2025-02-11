using SunnyHillStore.Core.ApplicationDbContexts;
using SunnyHillStore.Core.Repositories.Base;
using SunnyHillStore.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SunnyHillStore.Core.Services.CurrentUser;

namespace SunnyHillStore.Core.Repositories.Users
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context, ICurrentUserHelper currentUserHelper) : base(context, currentUserHelper)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted);
        }

        public async Task<User> GetByPasswordResetTokenAsync(string resetToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == resetToken && !u.IsDeleted);
        }
    }
}
