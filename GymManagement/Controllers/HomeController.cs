using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnalyticsService _analyticsService;

        public HomeController(ILogger<HomeController> logger, IAnalyticsService analyticsService)
        {
            _logger = logger;
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var res = await _analyticsService.GetDataAsync(ct);
            return View(res);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
