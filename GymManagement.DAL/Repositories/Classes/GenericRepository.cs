using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymManagement.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _set;
        public GenericRepository(GymDbContext context)
        {
            _dbContext = context;
            _set = _dbContext.Set<TEntity>();
        }

        public void AddAsync(TEntity entity)
        {
            _set.Add(entity);
           
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await _set.AsNoTracking().AnyAsync(predicate, ct);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        {
            return predicate is null ? await _dbContext.Set<TEntity>().AsNoTracking().CountAsync(ct) : await _dbContext.Set<TEntity>().AsNoTracking().CountAsync(predicate, ct);
        }

        public void DeleteAsync(TEntity entity)
        {
            _set.Remove(entity);
     
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            return await query.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool isTracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = isTracking ? _set : _set.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _set.FindAsync(id, ct);

        public void UpdateAsync(TEntity entity)
        {
            _set.Update(entity);
           
        }
    }
}
