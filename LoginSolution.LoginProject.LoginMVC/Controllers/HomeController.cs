using LoginSolution.LoginProject.BusinessLogic.Abstract;
using LoginSolution.LoginProject.LoginMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace LoginSolution.LoginProject.LoginMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUserService userService; 
        private ILogService logService;

        public HomeController(ILogger<HomeController> logger, IUserService _userService, ILogService _logService)
        {
            userService = _userService;
            logService = _logService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
               
        [Authorize]
        public IActionResult List()
        {
            ViewBag.RegisteredUserCount = TempData["RegisteredUserCount"]?.ToString();
            ViewBag.UnactivatedUserCount = userService.UnactivatedUserCount();
            ViewBag.AverageTimeCompleteActivate = TempData["AverageTimeCompleteActivate"]?.ToString();
            
            return View();
        }

        [HttpPost]
        public IActionResult List(DateTime startDate, DateTime endDate, DateTime date)
        {
            TempData["RegisteredUserCount"] = userService.GetRegisteredUserCount(startDate, endDate.AddDays(1)).ToString(); 
            TempData["AverageTimeCompleteActivate"] = userService.AverageTimeCompleteActivate(date).ToString(); 

            return RedirectToAction("List");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}