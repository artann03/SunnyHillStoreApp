using SunnyHillStore.Core.Repositories.Base;
using SunnyHillStore.Model.Entities.Base;

namespace SunnyHillStore.Core.Services.Base
{
    public class BaseService<T> : IBaseService<T> where T : BaseEntity
    {
        protected readonly IBaseRepository<T> _repository;

        public BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            if (id == default)
                throw new ArgumentNullException(nameof(id));

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Entity with id {id} not found");

            return entity;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _repository.CreateAsync(entity);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingEntity = await _repository.GetByIdAsync(entity.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Entity with id {entity.Id} not found");

            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            if (id == default)
                throw new ArgumentNullException(nameof(id));

            return await _repository.DeleteAsync(id);
        }
    }
}
