using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace GymManagement.BLL.Services.Classes
{
    public class PlansService : IPlansService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlansService(IUnitOfWork unitOfWork, IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(false, ct:ct);
            if (plans is null) return Result<IEnumerable<PlanViewModel>>.Ok([]);

            var plansModel = _mapper.Map<IEnumerable<Plan>, IEnumerable<PlanViewModel>>(plans);

            return Result<IEnumerable<PlanViewModel>>.Ok(plansModel);
        }

        public async Task<Result<PlanDetailsViewModel>> GetPlanDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return Result<PlanDetailsViewModel>.NotFound($"Plan With Id {id} Not Found!");

            var model = _mapper.Map<Plan, PlanDetailsViewModel>(plan);

            return Result<PlanDetailsViewModel>.Ok(model);
        }

        public async Task<Result<PlanToUpdateViewModel>> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null || !plan.IsActive) return Result<PlanToUpdateViewModel>.NotFound($"Plan With Id {id} Is Not Found Or Is Inactive!");
            if (await HasActiveMembershipsAsync(id, ct)) return Result<PlanToUpdateViewModel>.Fail($"Plan With Id {id} Has Active Members!");

            var model = _mapper.Map<Plan, PlanToUpdateViewModel>(plan);

            return Result<PlanToUpdateViewModel>.Ok(model);
        }

        public async Task<Result> UpdatePlanAsync(int id, PlanToUpdateViewModel updatedPlan, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null || !plan.IsActive) return Result.Fail($"Plan With Id {id} Is Not Found Or Is Inactive!");
            if (await HasActiveMembershipsAsync(id, ct)) return Result.Fail($"Plan With Id {id} Has Active Members!");

            _mapper.Map<PlanToUpdateViewModel,Plan>(updatedPlan, plan);
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0 ? Result.Ok() : Result.Fail($"Failed To Update Plan With Id {id}!");
        }

        public async Task<Result> ToggleStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return Result.Fail($"Plan With Id {id} Is Not Found!");
            if (plan.IsActive && await HasActiveMembershipsAsync(id, ct)) return Result.Fail($"Cannot Deactivate Plan With Id {id}, Has Active Members!"); ;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0 ? Result.Ok() : Result.Fail($"Failed To Toggle Status For Plan With Id {id}!");
        }

        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<MemberPlans>().AnyAsync(x => x.PlanId == planId && x.EndDate > DateTime.Now, ct);
        }
    }
}
