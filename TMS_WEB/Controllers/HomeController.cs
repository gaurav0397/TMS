using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using TMS_WEB.Models;
using TMS_WEB.Services;
using TMS_WEB.Viewmodel;

namespace TMS_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly APIservice _apiService;


        public HomeController(ILogger<HomeController> logger, APIservice apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _apiService.GetEmployees();
            ViewBag.EmployeeList = new SelectList(employees, "Id", "EmployeeName");

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(Employee model)
        {
            

            HttpContext.Session.SetInt32("EmployeeId", model.Id);

            return RedirectToAction("Index", "WorkItem"); 
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
