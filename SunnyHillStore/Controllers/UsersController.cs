using Microsoft.AspNetCore.Mvc;
using SunnyHillStore.Controllers.Base;
using SunnyHillStore.Core.Services.Users;
using SunnyHillStore.Model.Entities;

namespace SunnyHillStore.Controllers
{
    public class UsersController : BaseController<User>
    {
        public UsersController(IUserService userService) : base(userService) { }
        [HttpGet]
        public override async Task<IActionResult> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }
        [HttpPut]
        public override async Task<IActionResult> UpdateAsync(int id, User product)
        {
            return await base.UpdateAsync(id, product);
        }
        [HttpDelete]
        public override async Task<IActionResult> DeleteAsync(int id)
        {
            return await base.DeleteAsync(id);
        }
    }
}
