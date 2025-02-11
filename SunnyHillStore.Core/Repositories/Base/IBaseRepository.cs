using SunnyHillStore.Model.Entities.Base;

namespace SunnyHillStore.Core.Repositories.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByPublicIdAsync(string publicId);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id);
    }
}
