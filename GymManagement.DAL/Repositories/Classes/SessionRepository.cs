using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetBookedSessionsCountAsync(int id, CancellationToken ct)
        => await _dbContext.MemberSessions.CountAsync(x => x.SessionId == id, ct);


        public async Task<Session?> GetSessionByIdWithIncludeAsync(int id, CancellationToken ct = default)
        => await _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id, ct);


        public async Task<IEnumerable<Session>> GetSessionsWithIncludeAsync(Expression<Func<Session, bool>>? condition = null, CancellationToken ct = default)
        => condition is null ?
              await _dbContext.Sessions.Include(x => x.Trainer).Include(x => x.Category).ToListAsync(ct)
            : await _dbContext.Sessions.Where(condition).Include(x => x.Trainer).Include(x => x.Category).ToListAsync(ct);

    }
}
