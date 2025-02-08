using Microsoft.AspNetCore.Mvc;
using SunnyHillStore.Core.Services.Base;
using SunnyHillStore.Model.Entities.Base;

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

        [HttpPost]
        public virtual async Task<IActionResult> CreateAsync(T entity)
        {
            try
            {
                var createdEntity = await _service.CreateAsync(entity);
                return Ok(createdEntity);
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> UpdateAsync(int id, T entity)
        {
            if (id != entity.Id)
                return BadRequest();

            var updatedEntity = await _service.UpdateAsync(entity);
            return Ok(updatedEntity);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync(int id)
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
