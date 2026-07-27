using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();

        public Task<int> SaveChangesAsync(CancellationToken ct = default);

        public ISessionRepository SessionRepository { get; }
        public IMembershipRepository MembershipRepository { get; }
    }
}
