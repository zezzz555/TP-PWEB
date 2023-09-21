using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental.Data;
using Rental.Models;
using Rental.ViewModels;
using System.Diagnostics;

namespace Rental.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_logger = logger;
			_context =context;
		}

		public IActionResult Index()
		{
            ViewBag.CategoriaId = new SelectList(_context.Categoria.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
            ViewBag.EmpresaId = new SelectList(_context.Empresa.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
            var pesquisa = new PesquisaVeiculoCategoriaEmpresaViewModel();
            return View(pesquisa);
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