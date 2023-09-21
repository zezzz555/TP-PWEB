using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental.Data;
using Rental.Models;
using Rental.ViewModels;
using System.Data;

namespace Rental.Controllers
{
    [Authorize(Roles ="Gestor,Admin")]
    public class DashboardController : Controller
    {
        // GET: DashboardController
        [Authorize(Roles ="Admin")]
        public IActionResult Admin()
        {
            return View();
        }
        [Authorize(Roles = "Gestor")]
        public IActionResult Gestor()
        {
            return View();
        }
        public IActionResult Tables()
        {
            return View();
        }
    }
}
