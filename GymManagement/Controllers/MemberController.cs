using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MemberController : Controller
    {
        // Dependency Injection for Member Service
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MemberController(
            IMemberService memberService,
            IAttachmentService attachmentService
            )
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Picture(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if (member is null || string.IsNullOrWhiteSpace(member.value.Photo)) return NotFound();
            var res = _attachmentService.GetFile("MembersPicture", member.value.Photo);
            if (res is null) return NotFound();
            return File(res.Value.stream, res.Value.contentType);

        }

        // GET baseUrl/Members/Index
        // Index - List of all members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetMembersAsync(ct);
            return View(members.value);
        }

        // GET baseUrl/Members/Create
        // Create - Create and show empty form

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST baseUrl/Members/Create {Member}
        // Create - Create Member after form submit 
        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await _memberService.CreateMemberAsync(model, ct);

            if (res.success)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = res.error;


            return RedirectToAction(nameof(Index), TempData);
        }

        // GET baseUrl/Members/MemberDetails/{id}
        // MemberDetails - Show one member's details 
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);

            if (!member.success)
            {
                TempData["ErrorMessage"] = member.error;
                RedirectToAction(nameof(Index), TempData);
            }
            return View(member.value);
        }

        // GET baseUrl/Members/HealthRecordDetails/{id}
        // HealthRecordDetails - show one member's details 
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct = default)
        {
            var healthRecord = await _memberService.GetMemberHealthRecordAsync(id, ct);
            if (!healthRecord.success)
            {
                TempData["ErrorMessage"] = healthRecord.error;
                return RedirectToAction(nameof(Index), TempData);
            }

            return View(healthRecord.value);
        }

        // GET baseUrl/Members/Edit/{id}
        // Edit - Create and show pre-filled form for edit

        [HttpGet]
        public async Task<IActionResult> EditMember([FromRoute] int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (!member.success)
            {
                TempData["ErrorMessage"] = member.error;
                return RedirectToAction(nameof(Index), TempData);
            }
            return View(member.value);
        }

        // POST baseUrl/Members/Edit {EditedMember}
        // Edit - Update Member after form submit 
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await _memberService.UpdateMemberAsync(id, model, ct);

            if (res.success)
                TempData["SuccessMessage"] = "Member Updated Successfully!";   
            else
                TempData["ErrorMessage"] = res.error;


            return RedirectToAction(nameof(Index), TempData);
        }

        // GET baseUrl/Members/Delete/{id}
        // Delete - Show Confirmation Form
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if (!member.success)
            {
                TempData["ErrorMessage"] = member.error;
                return RedirectToAction(nameof(Index), TempData);
            }

            return View();
        }

        // POST baseUrl/Members/Delete {Member}
        // DeleteConfirmed - Submit form for delete
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct = default)
        {
            var res = await _memberService.RemoveMemberAsync(id, ct);
            if (res.success)
                TempData["SuccessMessage"] = "Member Deleted Successfully!";
            else
                TempData["ErrorMessage"] = res.error;

            return RedirectToAction(nameof(Index), TempData);
        }

    }
}
