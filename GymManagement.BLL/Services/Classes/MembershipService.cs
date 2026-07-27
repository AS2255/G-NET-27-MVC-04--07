using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Result<IEnumerable<MembershipViewModel>> GetAllMemberships()
        {
            var memberships = _unitOfWork.MembershipRepository.GetAllWithMemberAndPlans();
            var mappedMemberships = _mapper.Map<IEnumerable<MemberPlans>, IEnumerable<MembershipViewModel>>(memberships);
            return Result<IEnumerable<MembershipViewModel>>.Ok(mappedMemberships);
        }

        public async Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            var memberExist = await MemberEixstsAsync(model.MemberId, ct);
            var activePlanExists = await PlanExistsAsync(model.PlanId, ct);
            var hasActiveMembership = await HasActiveMembershipAsync(model.MemberId, ct);
            if (!memberExist || !activePlanExists || hasActiveMembership) return Result.Fail($"Failed To Create Membership! Member With Id {model.MemberId} Might Not Exist, Not Have An Active Membership Or Might Already Have An Active Plan!");

            var memberShipRepo = _unitOfWork.GetRepository<MemberPlans>();
            var memberShipToCreate = _mapper.Map<CreateMembershipViewModel,MemberPlans>(model);

            memberShipToCreate.CreatedAt = DateTime.Now;

            //Get plan, then get 'Duration' to add it to the expiration date for the created memebership.
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, ct);
            memberShipToCreate.EndDate = memberShipToCreate.CreatedAt.AddDays(plan!.DurationDays);

            memberShipRepo.AddAsync(memberShipToCreate);

            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed To Create Membership!");
        }

        public async Task<Result> DeleteMembershipAsync(int id, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<MemberPlans>();
            //get membership that has the member's id to delete it.
            var membership = await repo.FirstOrDefaultAsync(x => x.MemberId == id, true ,ct:ct);
            if (membership is null) return Result.NotFound($"Failed To Remove Membership With Id {id}, Not Found!");

            repo.DeleteAsync(membership);
            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail($"Failed To Remove Membership With Id {id}");
        }

        public async Task<Result<IEnumerable<PlanSelectListViewModel>>> GetPlansForDropDownAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);
            var activePlans = plans.Where(x => x.IsActive == true);
            if (activePlans is null)
                return Result<IEnumerable<PlanSelectListViewModel>>.Ok([]);

            var mappedPlans = _mapper.Map<IEnumerable<PlanSelectListViewModel>>(activePlans);
            return Result<IEnumerable<PlanSelectListViewModel>>.Ok(mappedPlans);
        }

        public async Task<Result<IEnumerable<MemberSelectListViewModel>>> GetMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct:ct);
            if (members is null)
                return Result<IEnumerable<MemberSelectListViewModel>>.Ok([]);

            var mappedMembers = _mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
            return Result<IEnumerable<MemberSelectListViewModel>>.Ok(mappedMembers);
        }


        // Helper Methods

        //Check if plan exists or not
        private async Task<bool> PlanExistsAsync(int id, CancellationToken ct = default) => await _unitOfWork.GetRepository<Plan>().AnyAsync(x => x.Id == id && x.IsActive == true, ct);

        //Check if member exists or not
        private async Task<bool> MemberEixstsAsync(int id, CancellationToken ct = default) => await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Id == id, ct);      

        //Check if member has any active plans
        private async Task<bool> HasActiveMembershipAsync(int memberId, CancellationToken ct = default) => await _unitOfWork.GetRepository<MemberPlans>().AnyAsync(x => x.MemberId == memberId && x.EndDate > DateTime.Now, ct);
    }
}
