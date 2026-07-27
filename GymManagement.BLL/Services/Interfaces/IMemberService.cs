using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.HealthRecordViewModels;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        public Task<Result<IEnumerable<MemberViewModel>>> GetMembersAsync(CancellationToken ct = default);
        public Task<Result> CreateMemberAsync(CreateMemberViewModel member, CancellationToken ct = default);
        public Task<Result<MemberViewModel>> GetMemberDetailsAsync(int id, CancellationToken ct = default);
        public Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int id, CancellationToken ct = default);
        public Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int id, CancellationToken ct = default);
        public Task<Result> UpdateMemberAsync(int id, MemberToUpdateViewModel updatedMember, CancellationToken ct);
        public Task<Result> RemoveMemberAsync(int id, CancellationToken ct = default);
    }
}
