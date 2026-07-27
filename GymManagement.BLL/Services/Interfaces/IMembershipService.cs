using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Result<IEnumerable<MembershipViewModel>> GetAllMemberships();
        Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken ct = default);
        Task<Result> DeleteMembershipAsync(int id, CancellationToken ct = default);
        Task<Result<IEnumerable<PlanSelectListViewModel>>> GetPlansForDropDownAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<MemberSelectListViewModel>>> GetMembersForDropDownAsync(CancellationToken ct = default);
    }
}
