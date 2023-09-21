using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental.Data;
using Rental.Models;
using Rental.Data.Migrations;
using Rental.ViewModels;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Rental.Controllers
{
    public class EstadoVeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EstadoVeiculosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EstadoVeiculos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EstadoVeiculos.Include(e => e.funcionario).Include(e => e.reserva);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EstadoVeiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EstadoVeiculos == null)
            {
                return NotFound();
            }

            var estadoVeiculo = await _context.EstadoVeiculos
                .Include(e => e.funcionario)
                .Include(e => e.reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estadoVeiculo == null)
            {
                return NotFound();
            }

            return View(estadoVeiculo);
        }

        public async Task<IActionResult> EntregaAoCliente(int? Id, [Bind("Id,Kms,Danos,Observações,Eliminado,ReservaId,FuncionarioId")] EstadoVeiculo estadoVeiculo)
        {
            if (Id != null)
            {
                var reserva = await _context.Reserva.Include(r => r.cliente).Include(r => r.veiculo).Where(e => e.Eliminado == false).FirstOrDefaultAsync(m => m.Id == Id);
                if (reserva != null)
                {
                    var kms = reserva.veiculo.Km;
                    estadoVeiculo.Kms = kms;
                    estadoVeiculo.reserva = reserva;
                    estadoVeiculo.ReservaId = reserva.Id;
                }
                var userId = _userManager.GetUserId(User);
                var utilizador = _context.Users.Find(userId);
                if (utilizador != null)
                {
                    estadoVeiculo.FuncionarioId = utilizador.Id;
                    estadoVeiculo.funcionario = utilizador;
                }
            }
            return View(estadoVeiculo);
        }

        public async Task<IActionResult> ReceberDoCliente(int? Id, [Bind("Id,Kms,Danos,Observações,Eliminado,ReservaId,FuncionarioId")] EstadoVeiculo estadoVeiculo, [FromForm] List<IFormFile> ficheiros)
        {
            //ficheiros upload danos
            if (Id != null)
            {
                string CoursePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload\\" + Id.ToString());
                if (Directory.Exists(CoursePath))
                {
                    var files = from file in Directory.EnumerateFiles(CoursePath)
                                select string.Format("\\Upload\\{0}\\{1}", Id, Path.GetFileName(file));
                    ViewData["Ficheiros"] = files;
                }

                //diretorio a guardar os ficheiros, wwwroot/upload/(id reserva)/(nome foto)
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string ReservaPath = Path.Combine(Directory.GetCurrentDirectory
                    (), "wwwroot\\Upload\\" + Id.ToString());
                if (!Directory.Exists(ReservaPath))
                {
                    Directory.CreateDirectory(ReservaPath);
                }
                foreach (var formFile in ficheiros)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.Combine(ReservaPath, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                        while (System.IO.File.Exists(filePath))
                        {
                            filePath = Path.Combine(ReservaPath, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                        }
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }

                var reserva = await _context.Reserva.Include(r => r.cliente).Include(r => r.veiculo).Where(e => e.Eliminado == false).FirstOrDefaultAsync(m => m.Id == Id);
                if (reserva != null)
                {
                    var kms = reserva.veiculo.Km;
                    estadoVeiculo.Kms = kms + estadoVeiculo.Kms;
                    estadoVeiculo.reserva = reserva;
                    estadoVeiculo.ReservaId = reserva.Id;


                }
                var userId = _userManager.GetUserId(User);
                var utilizador = _context.Users.Find(userId);
                if (utilizador != null)
                {
                    estadoVeiculo.FuncionarioId = utilizador.Id;
                    estadoVeiculo.funcionario = utilizador;
                }
            }
            return View(estadoVeiculo);
        }


        // POST: EstadoVeiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Kms,Danos,Observações,Eliminado,ReservaId,FuncionarioId")] EstadoVeiculo estadoVeiculo)
        {
            ModelState.Remove(nameof(estadoVeiculo.funcionario));
            ModelState.Remove(nameof(estadoVeiculo.FuncionarioId));
            if (ModelState.IsValid)
            {
                _context.Add(estadoVeiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(estadoVeiculo);
        }

        // GET: EstadoVeiculos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EstadoVeiculos == null)
            {
                return NotFound();
            }

            var estadoVeiculo = await _context.EstadoVeiculos.FindAsync(id);
            if (estadoVeiculo == null)
            {
                return NotFound();
            }
            ViewData["FuncionarioId"] = new SelectList(_context.Users, "Id", "Id", estadoVeiculo.FuncionarioId);
            ViewData["ReservaId"] = new SelectList(_context.Reserva, "Id", "Id", estadoVeiculo.ReservaId);
            return View(estadoVeiculo);
        }

        // POST: EstadoVeiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Kms,Danos,Observações,Eliminado,ReservaId,FuncionarioId")] EstadoVeiculo estadoVeiculo)
        {
            if (id != estadoVeiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estadoVeiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadoVeiculoExists(estadoVeiculo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FuncionarioId"] = new SelectList(_context.Users, "Id", "Id", estadoVeiculo.FuncionarioId);
            ViewData["ReservaId"] = new SelectList(_context.Reserva, "Id", "Id", estadoVeiculo.ReservaId);
            return View(estadoVeiculo);
        }

        // GET: EstadoVeiculos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EstadoVeiculos == null)
            {
                return NotFound();
            }

            var estadoVeiculo = await _context.EstadoVeiculos
                .Include(e => e.funcionario)
                .Include(e => e.reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estadoVeiculo == null)
            {
                return NotFound();
            }

            return View(estadoVeiculo);
        }

        // POST: EstadoVeiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EstadoVeiculos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EstadoVeiculos'  is null.");
            }
            var estadoVeiculo = await _context.EstadoVeiculos.FindAsync(id);
            if (estadoVeiculo != null)
            {
                //_context.EstadoVeiculos.Remove(estadoVeiculo);
                estadoVeiculo.Eliminado= false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadoVeiculoExists(int id)
        {
            return _context.EstadoVeiculos.Any(e => e.Id == id);
        }

        public IActionResult ApagarUpload(int id, string image)
        {

            if (id != 0 && !String.IsNullOrEmpty(image) && _context.Reserva != null)
            {
                var curso = _context.Reserva.FirstOrDefaultAsync(x => x.Id == id);
                if (curso == null)
                {
                    return NotFound();
                }
                string filePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot" + image);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

            }
            else
                return NotFound();

            return RedirectToAction(nameof(Edit), new { id = id });

        }

    }
}
