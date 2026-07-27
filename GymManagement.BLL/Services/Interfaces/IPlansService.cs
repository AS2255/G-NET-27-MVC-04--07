using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlansService
    {
        public Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct = default);
        public Task<Result<PlanDetailsViewModel>> GetPlanDetailsAsync(int id, CancellationToken ct = default);
        public Task<Result<PlanToUpdateViewModel>> GetPlanToUpdateAsync(int id, CancellationToken ct = default);
        public Task<Result> UpdatePlanAsync(int id, PlanToUpdateViewModel updatedPlan, CancellationToken ct = default);
        public Task<Result> ToggleStatusAsync(int id, CancellationToken ct = default);

    }
}
