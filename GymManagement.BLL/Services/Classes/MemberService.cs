using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.HealthRecordViewModels;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }
        public async Task<Result<IEnumerable<MemberViewModel>>> GetMembersAsync(CancellationToken ct)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return Result<IEnumerable<MemberViewModel>>.Ok([]); //if no members found return empty list.

            //METHOD1: using foreach, creating new MemberViewModel() object 
            //         for each member in members.

            //var membersViewModel = new List<MemberViewModel>();
            //foreach (var member in members)
            //{
            //    var memberViewModel = new MemberViewModel()
            //    {
            //        Name = member.Name,
            //        Email = member.Email,
            //        Gender = member.Gender.ToString(),
            //        Phone = member.Phone,
            //        Photo = member.Photo,
            //        id = member.Id
            //    };
            //    membersViewModel.Add(memberViewModel);
            //}


            //METHOD2: using LINQ Select to project each member into MemberViewModel object
            //         more readable and cleaner.

            //var membersViewModel = members.Select(
            //    x => new MemberViewModel()
            //    {
            //        Name = x.Name,
            //        Email = x.Email,
            //        Gender = x.Gender.ToString(),
            //        Phone = x.Phone,
            //        Photo = x.Photo,
            //        id = x.Id
            //    }).ToList();

            //METHOD3: using auto mapper
            var membersViewModel = _mapper.Map<IEnumerable<Member>,IEnumerable<MemberViewModel>>(members);

            return Result<IEnumerable<MemberViewModel>>.Ok(membersViewModel);
        }
        public async Task<Result> CreateMemberAsync(CreateMemberViewModel member, CancellationToken ct = default)
        {
            //Logic  Create Member To Database

            //Check Email Unique
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == member.Email, ct);
            //Checking Phone Number Unique
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == member.Phone, ct);

            if (emailExists || phoneExists) return Result.Fail("Cannot Create Member, Email Or Phone Number Already Exists!");

            var fileName = await _attachmentService.UploadAsync(member.PhotoFile.OpenReadStream(), "MembersPicture", member.PhotoFile.FileName, ct);
            if (string.IsNullOrWhiteSpace(fileName)) return Result.Fail("Cannot Upload the photo");

            var createdMember = _mapper.Map<CreateMemberViewModel, Member>(member);

            //Add To Database
            createdMember.Photo = fileName;
            _unitOfWork.GetRepository<Member>().AddAsync(createdMember);
            var rowsAffected = await _unitOfWork.SaveChangesAsync(ct);
            if (rowsAffected > 0)
            {
                return Result.Ok();
            }
            else
            {
                // Delete Photo Or File
                _attachmentService.Delete("MembersPicture", fileName);
                return Result.Fail("Failed To Create Member!");
            } 

        }
        public async Task<Result<MemberViewModel>> GetMemberDetailsAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result<MemberViewModel>.NotFound($"Member With Id {id} Is Not Found!");

            var model = _mapper.Map<Member, MemberViewModel>(member);

            var activeMembership = await _unitOfWork.GetRepository<MemberPlans>().FirstOrDefaultAsync(x => x.MemberId == member.Id && x.EndDate > DateTime.Now);
           
            if(activeMembership is not null)
            {
                var currentPlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct);

                model.PlanName = currentPlan?.Name;
                model.MemberShipStartDate = activeMembership.CreatedAt.ToString();
                model.MemberShipEndDate = activeMembership.EndDate.ToString();
            }

            return Result<MemberViewModel>.Ok(model);
        }
        public async Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.MemberId == id, ct: ct);
            if (healthRecord is null) return Result<HealthRecordViewModel>.NotFound($"Health Record With Id {id} Is Not Found!");

            var model = _mapper.Map<HealthRecord, HealthRecordViewModel>(healthRecord);

            return Result<HealthRecordViewModel>.Ok(model);
        }
        public async Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return Result<MemberToUpdateViewModel>.NotFound($"Member With Id {id} Is Not Found!");

            var model = _mapper.Map<Member, MemberToUpdateViewModel>(member);

            return Result<MemberToUpdateViewModel>.Ok(model);
        }
        public async Task<Result> UpdateMemberAsync(int id,MemberToUpdateViewModel updatedMember, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return Result.NotFound($"Failed To Update Member With Id {id}, Does Not Exist!");

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == updatedMember.Email && x.Id != id, ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == updatedMember.Phone && x.Id != id, ct);

            if (emailExists || phoneExists) return Result.Fail($"Failed To Update Member With Id {id}, Email Or Phone Number Already Exists!");

            _mapper.Map(updatedMember, member);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().UpdateAsync(member);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0 ? Result.Ok() : Result.Fail($"Failed To Update Member With Id {id}!");
        }
        public async Task<Result> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound($"Failed To Remove Member With Id {id}, Does Not Exist!");

            var hasFutureBooking = await _unitOfWork.GetRepository<MemberSessions>().AnyAsync(x => x.MemberId == id && x.Session.StartDate > DateTime.Now);
            if (hasFutureBooking) return Result.Fail($"Failed To Remove Member With Id {id}, Has Future Booking!");

            _unitOfWork.GetRepository<Member>().DeleteAsync(member);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            if (res > 0)
            {
                // Delete the photo from wwwroot/MembersPicture
                if (!string.IsNullOrWhiteSpace(member.Photo))
                {
                    _attachmentService.Delete("MembersPicture", member.Photo);
                }
                return Result.Ok();
            }
            else 
            { 
                return Result.Fail($"Failed To Remove Member With Id {id}!"); 
            }
        }

    }
}
