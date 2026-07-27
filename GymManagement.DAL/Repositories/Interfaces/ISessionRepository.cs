using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetSessionsWithIncludeAsync(CancellationToken ct = default);
        Task<Session?> GetSessionByIdWithIncludeAsync(int id, CancellationToken ct = default);
        Task<int> GetBookedSessionsCountAsync(int id, CancellationToken ct);
    }
}
