using SunnyHillStore.Core.Repositories.Users;
using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Core.Services.Users
{
    public class UserService : BaseService<User>, IUserService
    {
        public UserService(IUserRepository userRepository): base(userRepository) { }
    }
}
