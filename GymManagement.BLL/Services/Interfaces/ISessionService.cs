using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionsAsync(CancellationToken ct = default);
        Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct = default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);
        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);
        Task<Result> DeleteSessionAsync(int id, CancellationToken ct = default);
        Task<Result<IEnumerable<TrainerSelectViewModel>>> GetTrainerSelectListAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<CategorySelectViewModel>>> GetCategorySelectListAsync(CancellationToken ct = default);

    }
}
