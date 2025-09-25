using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pedidosApp.Models;
using System.Diagnostics;

namespace pedidosApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Si el usuario ya está logueado, mostrar dashboard
            if (User.Identity.IsAuthenticated)
            {
                return View("Dashboard");
            }

          
            return View();
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
