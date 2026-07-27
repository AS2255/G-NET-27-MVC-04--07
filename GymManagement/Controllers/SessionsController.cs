using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // Action to display all sessions
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);

            return View(sessions.value);
        }

        // Action to create a new session
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct = default)
        {
            await PopulateAllDropDownAsync(ct);
            return View();
        }

        // Action to handle the form submission for creating a new session
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateAllDropDownAsync(ct);
                View(model);
            }


            var res = await _sessionService.CreateSessionAsync(model, ct);

            if (res.success)
            {
                TempData["SuccessMessage"] = "Session Created Successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = res.error;
                await PopulateAllDropDownAsync(ct);
                return View(model);
            }

        }

        // Action to display the details of a specific session
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var res = await _sessionService.GetSessionByIdAsync(id, ct);
            if (!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(res.value);
            }
        }

        // Action to display the edit form for a specific session
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var res = await _sessionService.GetSessionToUpdateAsync(id, ct);

            if (!res.success)
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                await PopulateTrainersDropDownAsync(ct);
                return View(res.value);
            }
        }

        // Action to handle the form submission for editing a specific session
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateTrainersDropDownAsync(ct);
                return View(model);
            }

            var res = await _sessionService.UpdateSessionAsync(id, model, ct);

            if (res.success)
            {
                TempData["SuccessMessage"] = "Session Updated";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = res.error;
                await PopulateTrainersDropDownAsync(ct);
                return View(model);
            }
        }

        // Action to display the delete confirmation page for a specific session
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var res = await _sessionService.GetSessionByIdAsync(id, ct);

            if(res.success)
            {
                return View(res.value);
            }
            else
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
        }

        // Action to handle the deletion of a specific session
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var res = await _sessionService.DeleteSessionAsync(id, ct);
            if(res.success)
            {
                TempData["SuccessMessage"] = "Session Deleted Successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = res.error;
                return RedirectToAction(nameof(Index));
            }
        }

        // Helper method to populate dropdown lists for categories and trainers
        private async Task PopulateAllDropDownAsync(CancellationToken ct = default)
        {
            var categoreis = await _sessionService.GetCategorySelectListAsync(ct);
            ViewBag.Categories = new SelectList(categoreis.value, "Id", "CategoryName");

            var trainers = await _sessionService.GetTrainerSelectListAsync(ct);
            ViewBag.Trainers = new SelectList(trainers.value, "Id", "Name");
        }

        // Helper method to populate the trainers dropdown list
        private async Task PopulateTrainersDropDownAsync(CancellationToken ct = default)
        {

            var trainers = await _sessionService.GetTrainerSelectListAsync(ct);
            ViewBag.Trainers = new SelectList(trainers.value, "Id", "Name");
        } 
    }
}
