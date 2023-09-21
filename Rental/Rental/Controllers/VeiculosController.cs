using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Rental.Data;
using Rental.Models;
using Rental.ViewModels;

namespace Rental.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VeiculosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Veiculos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Veiculo.Include(v => v.categoria).Include(v => v.empresa).Where(v => v.Eliminado == false);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Veiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo
               .Include(v => v.categoria)
               .Include(v => v.empresa)
               .Include(v => v.reservas)
               .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        // GET: Veiculos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Set<Categoria>(), "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Set<Empresa>(), "Id", "Nome");
            return View();
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,Marca,Modelo,Preco,Localizacao,FotoURL,Km,Transmissao,TipoCombustivel,NumPortas,Deposito,Estado,Eliminado,CategoriaId,EmpresaId")] Veiculo veiculo)
        {
            ModelState.Remove(nameof(veiculo.categoria));
            ModelState.Remove(nameof(veiculo.empresa));
            if (ModelState.IsValid)
            {
                _context.Add(veiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Catalogo));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Set<Categoria>(), "Id", "Nome", veiculo.CategoriaId);
            ViewData["EmpresaId"] = new SelectList(_context.Set<Empresa>(), "Id", "Nome", veiculo.EmpresaId);
            return View(veiculo);
        }

        // GET: Veiculos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo
               .Include(v => v.categoria)
               .Include(v => v.empresa)
               .Include(v => v.reservas)
               .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Set<Categoria>(), "Id", "Nome", veiculo.CategoriaId);
            ViewData["EmpresaId"] = new SelectList(_context.Set<Empresa>(), "Id", "Nome", veiculo.EmpresaId);
            return View(veiculo);
        }

        // POST: Veiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Marca,Modelo,Preco,Localizacao,FotoURL,Km,Transmissao,TipoCombustivel,NumPortas,Deposito,Estado,Eliminado,CategoriaId,EmpresaId")] Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(veiculo.categoria));
            ModelState.Remove(nameof(veiculo.empresa));
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Catalogo));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Set<Categoria>(), "Id", "Nome", veiculo.CategoriaId);
            ViewData["EmpresaId"] = new SelectList(_context.Set<Empresa>(), "Id", "Nome", veiculo.EmpresaId);
            return View(veiculo);
        }

        // GET: Veiculos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo
                .Include(v => v.categoria)
                .Include(v => v.empresa)
                .Include(v => v.reservas)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        // POST: Veiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Veiculo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Veiculo'  is null.");
            }
            var veiculo = await _context.Veiculo
                .Include(v => v.categoria)
                .Include(v => v.empresa)
                .Include(v => v.reservas)
                .FirstOrDefaultAsync(m => m.Id == id);
            bool ReservaAtiva = false;
            //apenas elimina veiculos que não tenham reservas ativas
            if (veiculo != null)
            {
                if (veiculo.reservas != null)
                {
                    foreach (var reserva in veiculo.reservas)
                    {
                        if (reserva.Terminado == false && reserva.Eliminado == false)
                        {
                            ReservaAtiva = true;
                        }
                    }
                }
                if (ReservaAtiva == false)
                {
                    //_context.Veiculo.Remove(veiculo);
                    veiculo.Eliminado = true;
                }
                else if (ReservaAtiva == true)
                {
                    ModelState.AddModelError("", "Veiculo com reservas ativas, não foi possivel eliminar");
                    return RedirectToAction(nameof(Delete), id);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Catalogo));
        }

        private bool VeiculoExists(int id)
        {
            return _context.Veiculo.Any(e => e.Id == id);
        }

        // GET
        public IActionResult PesquisaInicial()
        {
            if (_context.Categoria != null && _context.Empresa != null)
            {
                ViewBag.CategoriaId = new SelectList(_context.Categoria.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
                ViewBag.EmpresaId = new SelectList(_context.Empresa.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
            }
            var pesquisa = new PesquisaVeiculoCategoriaEmpresaViewModel();
            return View(pesquisa);
        }

        [HttpPost]
        public async Task<IActionResult> PesquisaInicial([Bind("TipoVeiculo", "Localizacao", "DataLevantamento", "DataEntrega", "CategoriaId", "EmpresaId", "NumResultados", "Estado")] PesquisaVeiculoCategoriaEmpresaViewModel pesquisa)
        {
            ViewBag.CategoriaId = new SelectList(_context.Categoria.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
            ViewBag.EmpresaId = new SelectList(_context.Empresa.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");

            if (pesquisa.DataLevantamento > pesquisa.DataEntrega)
                ModelState.AddModelError("DataEntrega", "A data de entrega não pode ser antes da data de levantamento");
            if (pesquisa.DataLevantamento < DateTime.Now)
                ModelState.AddModelError("DataLevantamento", "A data de levantamento não pode ser no passado");
            if (pesquisa.DataLevantamento.Date == pesquisa.DataEntrega.Date)
                ModelState.AddModelError("DataLevantamento", "Tem de alugar o veículo por pelo menos 1 dia");
            if (ModelState.IsValid == false)
                return View(pesquisa);
            if (_context.Categoria != null && _context.Empresa != null)
            {
                if (pesquisa.CategoriaId != null)
                {
                    var categoria = _context.Categoria.Find(pesquisa.CategoriaId);
                    if (categoria != null)
                        pesquisa.CategoriaId = categoria.Id;
                }
                if (pesquisa.EmpresaId != null)
                {
                    var empresa = _context.Empresa.Find(pesquisa.EmpresaId);
                    if (empresa != null)
                        pesquisa.EmpresaId = empresa.Id;
                }
            }
            //Cria lista com todos os veiculos ativos
            var veiculos = await _context.Veiculo.Where(v => v.Eliminado == false && v.Estado == true).Include(r => r.reservas).ToListAsync();
            //Cria lista com todos os veiculos disponiveis (sem reservas nas datas selecionadas)
            var VeiculosDisponiveis = ListaVeiculosDisponiveis(veiculos, pesquisa.DataLevantamento, pesquisa.DataEntrega);
            pesquisa.ListaDeVeiculos = new List<Veiculo>();
            //filtrar por Localizacao
            if (!string.IsNullOrWhiteSpace(pesquisa.Localizacao))
                pesquisa.ListaDeVeiculos.AddRange(VeiculosDisponiveis.Where(v => v.Localizacao.ToLower().Contains(pesquisa.Localizacao.ToLower())));
            //filtrar por Tipo de veiculo
            if (!string.IsNullOrWhiteSpace(pesquisa.TipoVeiculo))
            {
                List<Veiculo> VeiculosPesquisa = new();
                VeiculosPesquisa.AddRange(VeiculosDisponiveis.Where(v => v.Tipo.ToLower().Contains(pesquisa.TipoVeiculo.ToLower())));
                pesquisa.ListaDeVeiculos = VeiculosPesquisa.Intersect(pesquisa.ListaDeVeiculos).ToList();

            }
            //filtrar por categoria
            if (pesquisa.CategoriaId != null)
            {
                List<Veiculo> VeiculosPesquisa = new();
                VeiculosPesquisa.AddRange(VeiculosDisponiveis.Where(v => v.CategoriaId.Equals(pesquisa.CategoriaId)));
                pesquisa.ListaDeVeiculos = VeiculosPesquisa.Intersect(pesquisa.ListaDeVeiculos).ToList();
            }
            //filtrar por empresa
            if (pesquisa.EmpresaId != null)
            {
                List<Veiculo> VeiculosPesquisa = new();
                VeiculosPesquisa.AddRange(VeiculosDisponiveis.Where(v => v.EmpresaId.Equals(pesquisa.EmpresaId)));
                pesquisa.ListaDeVeiculos = VeiculosPesquisa.Intersect(pesquisa.ListaDeVeiculos).ToList();
            }
            pesquisa.NumResultados = pesquisa.ListaDeVeiculos.Count;
            return View(pesquisa);
        }

        //GET  Listar a frota de veículos da empresa
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> Catalogo()
        {
            if (_context.Categoria != null)
                ViewBag.CategoriaId = new SelectList(_context.Categoria.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
            var pesquisa = new PesquisaVeiculoCategoriaEmpresaViewModel();
            //verifica a empresa a que o user pertence e mostra apenas os resultados dessa empresa
            var userId = _userManager.GetUserId(User);
            var utilizador = _context.Users.Find(userId);
            if (utilizador != null)
            {
                var empresa = await _context.Empresa.FindAsync(utilizador.EmpresaId);
                if (empresa != null)
                {
                    var Veiculos = await _context.Veiculo.Where(v => v.Eliminado == false && v.EmpresaId == utilizador.EmpresaId).ToListAsync();
                    pesquisa.ListaDeVeiculos = Veiculos;
                    return View(pesquisa);
                }
            }
            return View(pesquisa);
        }

        [HttpPost]
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> Catalogo([Bind("TipoVeiculo", "Localizacao", "DataLevantamento", "DataEntrega", "CategoriaId", "EmpresaId", "NumResultados", "Estado")] PesquisaVeiculoCategoriaEmpresaViewModel catalogo)
        {
            if (_context.Categoria != null)
                ViewBag.CategoriaId = new SelectList(_context.Categoria.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
            //verifica a empresa a que o user pertence e mostra apenas os resultados dessa empresa
            var userId = _userManager.GetUserId(User);
            var utilizador = _context.Users.Find(userId);
            if (utilizador != null)
            {
                var empresa = await _context.Empresa.FindAsync(utilizador.EmpresaId);
                if (catalogo.CategoriaId != null)
                {
                    var categoria = _context.Categoria.Find(catalogo.CategoriaId);
                    if (categoria != null)
                        catalogo.CategoriaId = categoria.Id;
                }
                if (empresa != null)
                {
                    var Veiculos = await _context.Veiculo.Where(v => v.Eliminado == false && v.EmpresaId == utilizador.EmpresaId).ToListAsync();
                    catalogo.ListaDeVeiculos = new List<Veiculo>();
                    //filtrar por categoria
                    if (catalogo.CategoriaId != null)
                    {
                        List<Veiculo> VeiculosPesquisa = new();
                        VeiculosPesquisa.AddRange(Veiculos.Where(v => v.CategoriaId.Equals(catalogo.CategoriaId)));
                        catalogo.ListaDeVeiculos.AddRange(VeiculosPesquisa);
                    }
                    //filtrar por estado
                    if (catalogo.Estado != null)
                    {
                        List<Veiculo> VeiculosPesquisa = new();
                        VeiculosPesquisa.AddRange(Veiculos.Where(v => v.Estado.Equals(catalogo.Estado)));
                        if (catalogo.CategoriaId == null)
                            catalogo.ListaDeVeiculos.AddRange(VeiculosPesquisa);
                        else
                            catalogo.ListaDeVeiculos = VeiculosPesquisa.Intersect(catalogo.ListaDeVeiculos).ToList();
                    }
                    if (catalogo.CategoriaId == null && catalogo.Estado == null)
                        catalogo.ListaDeVeiculos = Veiculos;
                    return View(catalogo);
                }
            }
            return View();
        }

        public static bool VeiculoDisponivel(Veiculo veiculo, DateTime dataLevantamento, DateTime dataEntrega)
        {
            if (veiculo.reservas != null)
            {
                foreach (var reserva in veiculo.reservas)
                {
                    //se o veiculo estiver reservado nas datas selecionadas e se essas reservas não tiverem sido eliminadas ou rejeitadas
                    if (!(dataEntrega.CompareTo(reserva.DataLevantamento) < 0 || dataLevantamento.CompareTo(reserva.DataEntrega) > 0) && reserva.Eliminado == false && reserva.Rejeitada == false)
                        return false;
                }
            }
            return true;
        }

        public List<Veiculo> ListaVeiculosDisponiveis(List<Veiculo> veiculos, DateTime dataLevantamento, DateTime dataEntrega)
        {
            List<Veiculo> VeiculosDisponiveis = new();
            foreach (var veiculo in veiculos)
            {
                if (veiculo.reservas != null && veiculo.reservas.Count != 0)
                {
                    bool reservado = false;
                    foreach (var reserva in veiculo.reservas)
                    {
                        if (!(dataEntrega.CompareTo(reserva.DataLevantamento) < 0 || dataLevantamento.CompareTo(reserva.DataEntrega) > 0) && reserva.Eliminado == false && reserva.Rejeitada == false)
                            reservado = true;
                    }
                    if (reservado == false)
                        VeiculosDisponiveis.Add(veiculo);
                }
                else
                {
                    VeiculosDisponiveis.Add(veiculo);
                }
            }
            return VeiculosDisponiveis;
        }
    }
}
