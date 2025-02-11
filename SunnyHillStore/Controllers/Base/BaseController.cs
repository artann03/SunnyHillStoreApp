using Microsoft.AspNetCore.Mvc;
using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities.Base;
using SunnyHillStore.Model.Models;
using SunnyHillStore.Model.Models.Products;

namespace SunnyHillStore.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : ControllerBase where T : BaseEntity
    {
        protected readonly IBaseService<T> _service;

        protected BaseController(IBaseService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAllAsync()
        {
            var entities = await _service.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _service.GetByIdAsync(id);
                return Ok(entity);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (result)
                    return NoContent();
                return NotFound();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
        }
    }
}
