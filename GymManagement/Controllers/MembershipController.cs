using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    public class MembershipController (IMembershipService membershipService) : Controller
    {
        // GET: Membership/Index
        [HttpGet]
        public IActionResult Index()
        {
            var memberships = membershipService.GetAllMemberships();

            return View(memberships.value);
        }

        // GET: Membership/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropDowns();
            return View();
        }

        // POST: Membership/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            if(!ModelState.IsValid)
            {
                await LoadDropDowns(ct);
                TempData["ErrorMessage"] = "Creation Failed, Check Data";
                return View(model);
            }

            var res = await membershipService.CreateMembershipAsync(model, ct);
            
            if(res.success)
            {
                TempData["SuccessMessage"] = "MemberShip Created Successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = res.error;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Membership/Cancel
        [HttpGet]
        public IActionResult Cancel()
        {
            return View();
        }

        // POST: Membership/Cancel
        [HttpPost]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct = default)
        {
           
            var res = await membershipService.DeleteMembershipAsync(id, ct);

            if (res.success)
            {
                TempData["SuccessMessage"] = "MemberShip Deleted Successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = res.error;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Membership/Details
        public async Task LoadDropDowns(CancellationToken ct = default)
        {
            var members = await membershipService.GetMembersForDropDownAsync(ct);
            var plans = await membershipService.GetPlansForDropDownAsync(ct);

            ViewBag.Members = new SelectList(members.value, "Id", "Name");
            ViewBag.Plans = new SelectList(plans.value, "Id", "Name");
        }
    }
}
