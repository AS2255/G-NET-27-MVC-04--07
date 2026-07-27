using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class TrainersController : Controller
    {

        private readonly ITrainersService _trainersService;
        public TrainersController(ITrainersService trainersService)
        {
            _trainersService = trainersService;
        }


        //GET: get all trainers 
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var trainers = await _trainersService.GetAllTrainersAsync();

            return View(trainers.value);
        }

        //GET: get form
        [HttpGet]
        public IActionResult Create(CancellationToken ct = default)
        {
            return View();
        }

        //POST: send trainer form
        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel trainer, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(trainer);

            var res = await _trainersService.CreateTrainerAsync(trainer, ct);

            if (res.success)
                TempData["SuccessMessage"] = "Trainer created successfully!";
            else
                TempData["ErrorMessage"] = res.error;

            return RedirectToAction(nameof(Index), TempData);
        }

        //GET: get trainer details
        public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct = default)
        {
            var trainer = await _trainersService.GetTrainerDetailsAsync(id, ct);

            if (!trainer.success)
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index), TempData);
            }
            else
            {
                return View(trainer.value);
            }
        }

        //GET: get trainer form to edit
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id, CancellationToken ct = default)
        {
            var trainer = await _trainersService.GetTrainerToUpdateAsync(id, ct);

            if (!trainer.success)
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index), TempData);
            }

            return View(trainer.value);
        }

        //POST: send trainer form to edit
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await _trainersService.UpdateTrainerAsync(id, model, ct);

            if (res.success)
                TempData["SuccessMessage"] = "Trainer updated successfully";
            else
                TempData["ErrorMessage"] = res.error;

            return RedirectToAction(nameof(Index), TempData);
        }

        //GET: get trainer form to delete
        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct = default)
        {
            var trainer = await _trainersService.GetTrainerDetailsAsync(id, ct);
            if (!trainer.success)
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index), TempData);
            }

            return View();
        }

        //POST: send trainer form to delete
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct = default)
        {
            var res = await _trainersService.RemoveTrainerAsync(id, ct);

            if (res.success)
                TempData["SuccessMessage"] = "Trainer removed successfully";
            else
                TempData["ErrorMessage"] = res.error;

            return RedirectToAction(nameof(Index), TempData);
        }
    }
}
