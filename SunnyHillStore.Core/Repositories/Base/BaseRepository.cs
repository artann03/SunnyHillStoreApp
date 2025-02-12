using Microsoft.EntityFrameworkCore;
using SunnyHillStore.Core.ApplicationDbContexts;
using SunnyHillStore.Core.Services.CurrentUser;
using SunnyHillStore.Model.Entities.Base;

namespace SunnyHillStore.Core.Repositories.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly ICurrentUserHelper _currentUserService;
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public BaseRepository(ApplicationDbContext context, ICurrentUserHelper currentUserService)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _currentUserService = currentUserService;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
        }

        public virtual async Task<T> GetByPublicIdAsync(string publicId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.PublicId == publicId && !x.IsDeleted);
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            entity.PublicId = Guid.NewGuid().ToString();
            entity.CreatedBy = _currentUserService.UserId;
            entity.UpdatedBy = _currentUserService.UserId;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByPublicIdAsync(id);
            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
