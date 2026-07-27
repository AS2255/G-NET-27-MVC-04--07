using GymManagement.BLL.ViewModels.AccountViewModels;
using GymManagement.Controllers;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // Get Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Post login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email Or Password!");
                return View(model);
            }

            var signinResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (signinResult.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} Is Signed In");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (signinResult.IsLockedOut)
            {
                _logger.LogWarning($"User {user.UserName} Is Locked Out");
                ModelState.AddModelError("InvalidLogin", "This Account Is Locked , Try Again Later");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email Or Password");
                return View(model);
            }

        }

        // Logout

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }


        // Access Denied

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
