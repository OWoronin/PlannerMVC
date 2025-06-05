using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using System.Diagnostics;

namespace Pz_Proj_11_12.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Users");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Forbid()
        {
            return View();
        }

        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }

        public IActionResult BadRequestPage()
        {
            return View("BadRequest");
        }

    }
}
