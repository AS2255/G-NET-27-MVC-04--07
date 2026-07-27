using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<MemberSessions>, IBookingRepository
    {
        private readonly GymDbContext _dbContext;

        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MemberSessions>> GetBookingsBySessionIdAsync(int sessionId, CancellationToken ct = default)
            => await _dbContext.MemberSessions.AsNoTracking().Include(x => x.Member).Where(ms => ms.SessionId == sessionId).ToListAsync(ct);

    }
}
