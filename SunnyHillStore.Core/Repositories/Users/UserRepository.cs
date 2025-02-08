using SunnyHillStore.Core.ApplicationDbContexts;
using SunnyHillStore.Core.Repositories.Base;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Core.Repositories.Users
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
