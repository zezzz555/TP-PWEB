using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Rental.Data;
using Rental.Models;
using Rental.ViewModels;

namespace Rental.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReservasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservas
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Index()
        {
            if (_context.Categoria != null && _context.Empresa != null)
            {
                ViewBag.CategoriaId = new SelectList(_context.Categoria.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
                ViewBag.ClienteId = new SelectList(await _userManager.GetUsersInRoleAsync("Cliente"), "Id", "UserName");
            }
            //verifica a empresa a que o user pertence e mostra apenas as reservas dessa empresa
            var userId = _userManager.GetUserId(User);
            var utilizador = _context.Users.Find(userId);
            if (utilizador != null)
            {
                var empresa = await _context.Empresa.FindAsync(utilizador.EmpresaId);
                if (empresa != null)
                {
                    var pesquisa = new ReservasCategoriaClienteDatasViewModel();
                    pesquisa.ListaDeReservas = _context.Reserva.Include(r => r.cliente).Include(r => r.veiculo).Where(e => e.Eliminado == false && e.veiculo.EmpresaId == utilizador.EmpresaId).ToList();
                    return View(pesquisa);
                }
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Index([Bind("CategoriaId,ClienteId,DataLevantamento,DataEntrega")] ReservasCategoriaClienteDatasViewModel pesquisa)
        {
            if (_context.Categoria != null && _context.Users != null)
            {
                ViewBag.CategoriaId = new SelectList( _context.Categoria.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
                ViewBag.ClienteId = new SelectList(await _userManager.GetUsersInRoleAsync("Cliente"), "Id", "UserName");
                var userId = _userManager.GetUserId(User);
                var utilizador = _context.Users.Find(userId);
                if (utilizador == null)
                    ModelState.AddModelError("", "User não existe");
                if (pesquisa.CategoriaId != null)
                {
                    var categoria =await _context.Categoria.FindAsync(pesquisa.CategoriaId);
                    if (categoria != null)
                        pesquisa.CategoriaId = categoria.Id;
                }
                if (pesquisa.ClienteId != null)
                {
                    var cliente = await _context.Users.FindAsync(pesquisa.ClienteId);
                    if (cliente != null)
                        pesquisa.ClienteId = cliente.Id;
                    else
                        ModelState.AddModelError("", "Cliente não existe");
                }
                var reservasExistentes = _context.Reserva.Include(r => r.cliente).Include(r => r.veiculo).Where(e => e.Eliminado == false && e.veiculo.EmpresaId == utilizador.EmpresaId).ToList();
                pesquisa.ListaDeReservas = new List<Reserva>();
                //filtrar por Data de Levantamento
                if (pesquisa.DataLevantamento != null)
                {
                    pesquisa.ListaDeReservas.AddRange(reservasExistentes.Where(v => v.DataLevantamento.Equals(pesquisa.DataLevantamento)));
                }
                //filtrar por Data de entrega
                if (pesquisa.DataEntrega != null)
                {
                    List<Reserva> ReservasPesquisa = new();
                    ReservasPesquisa.AddRange(reservasExistentes.Where(v => v.DataEntrega.Equals(pesquisa.DataEntrega)));
                    if (pesquisa.DataLevantamento != null)
                        pesquisa.ListaDeReservas = ReservasPesquisa.Intersect(pesquisa.ListaDeReservas).ToList();
                    else
                        pesquisa.ListaDeReservas = ReservasPesquisa;
                }
                //filtrar por categoria
                if (pesquisa.CategoriaId != null)
                {
                    List<Reserva> ReservasPesquisa = new();
                    ReservasPesquisa.AddRange(reservasExistentes.Where(v => v.veiculo.CategoriaId.Equals(pesquisa.CategoriaId)));
                    if (pesquisa.DataLevantamento != null || pesquisa.DataEntrega != null)
                        pesquisa.ListaDeReservas = ReservasPesquisa.Intersect(pesquisa.ListaDeReservas).ToList();
                    else
                        pesquisa.ListaDeReservas = ReservasPesquisa;
                }
                //filtrar por cliente
                if (pesquisa.ClienteId != null)
                {
                    List<Reserva> ReservasPesquisa = new();
                    ReservasPesquisa.AddRange(reservasExistentes.Where(v => v.ClienteId.Equals(pesquisa.ClienteId)));
                    if (pesquisa.DataLevantamento != null || pesquisa.DataEntrega != null || pesquisa.CategoriaId != null)
                        pesquisa.ListaDeReservas = ReservasPesquisa.Intersect(pesquisa.ListaDeReservas).ToList();
                    else
                        pesquisa.ListaDeReservas = ReservasPesquisa;
                }
                //sem filtros
                else if(pesquisa.DataLevantamento == null && pesquisa.DataEntrega == null && pesquisa.CategoriaId == null && pesquisa.ClienteId == null)
                {
                    pesquisa.ListaDeReservas = reservasExistentes;
                }
            }
            return View(pesquisa)
;
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.cliente)
                .Include(r => r.veiculo)
                .Where(e => e.Eliminado == false)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DataLevantamento,DataEntrega,Custo,DuracaoEmHoras,DuracaoEmMinutos,VeiculoId,ClienteId")] Reserva reserva)
        {
            ModelState.Remove(nameof(reserva.veiculo));
            ModelState.Remove(nameof(reserva.cliente));
            reserva.DataPedido = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AsMinhasReservas));
            }

            ViewData["ClienteId"] = new SelectList(_context.Users.Where(e => e.Eliminado == false), "Id", "Id", reserva.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo.Where(e => e.Eliminado == false), "Id", "Id", reserva.VeiculoId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Username", reserva.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Set<Veiculo>(), "Id", "Modelo", reserva.VeiculoId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataPedido,DataLevantamento,DataEntrega,DuracaoEmHoras,DuracaoEmMinutos,Custo,EmTransito,Rejeitada,Terminado,Eliminado,Confirmado,VeiculoId,ClienteId")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(reserva.cliente));
            ModelState.Remove(nameof(reserva.veiculo));
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Username", reserva.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Set<Veiculo>(), "Id", "Modelo", reserva.VeiculoId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.cliente)
                .Include(r => r.veiculo)
                .Where(e => e.Eliminado == false)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reserva == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reserva'  is null.");
            }
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva != null)
            {
                //_context.Reserva.Remove(reserva);
                reserva.Eliminado = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reserva.Any(e => e.Id == id);
        }

        //GET Criar Pedido de Reserva
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Pedido(int? Id, DateTime DataLevantamento, DateTime DataEntrega)
        {
            if (Id == null || _context.Veiculo == null)
            {
                return NotFound();
            }
            ViewData["Veiculo"] = new SelectList(_context.Veiculo.Where(e => e.Eliminado == false), "Id", "Modelo");
            ViewData["ListaClientes"] = new SelectList(_context.Users.Where(e => e.Eliminado == false), "Id", "PrimeiroNome");
            var pedido = new ReservaViewModel();
            pedido.VeiculoId = Id;
            pedido.DataLevantamento = DataLevantamento;
            pedido.DataEntrega = DataEntrega;
            var veiculo = await _context.Veiculo
            .Include(v => v.categoria)
            .Include(v => v.empresa)
            .Where(e => e.Eliminado == false)
            .FirstOrDefaultAsync(m => m.Id == Id);
            if (veiculo == null)
            {
                return NotFound();
            }
            pedido.veiculo = veiculo;
            return View(pedido);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public IActionResult Pedido([Bind("DataLevantamento", "DataEntrega", "VeiculoId")] ReservaViewModel pedido)
        {
            ViewData["Veiculo"] = new SelectList(_context.Veiculo.Where(e => e.Eliminado == false), "Id", "Modelo");
            ViewData["ListaClientes"] = new SelectList(_context.Users.Where(e => e.Eliminado == false), "Id", "PrimeiroNome");

            if (pedido.VeiculoId != 0)
            {
                var veiculo = _context.Veiculo.Find(pedido.VeiculoId);
                if (veiculo != null)
                {
                    if (VeiculoDisponivel(veiculo, pedido.DataLevantamento, pedido.DataEntrega) && veiculo.Estado == true)
                    {
                        pedido.veiculo = veiculo;
                        pedido.DuracaoEmDias = (pedido.DataEntrega - pedido.DataLevantamento).TotalDays;
                        pedido.DuracaoEmHoras = (pedido.DataEntrega - pedido.DataLevantamento).TotalHours;
                        pedido.DuracaoEmMinutos = (pedido.DataEntrega - pedido.DataLevantamento).TotalMinutes;
                        pedido.Custo = Math.Round(pedido.veiculo.Preco * pedido.DuracaoEmDias);
                        pedido.ClienteId = _userManager.GetUserId(User);
                        pedido.cliente = _context.Users.Find(pedido.ClienteId);
                    }
                    else
                        ModelState.AddModelError("VeiculoId", "Veiculo Indisponível");
                }
                else
                    ModelState.AddModelError("VeiculoId", "Veiculo inválido");
            }
            return View("PedidoConfirmacao", pedido);
        }

        [HttpPost]
        public IActionResult Calcular([Bind("DataLevantamento", "DataEntrega", "VeiculoId")] ReservaViewModel pedido)
        {
            ViewData["TipoDeAulaId"] = new SelectList(_context.Veiculo.Where(e => e.Eliminado == false), "Id", "Modelo");
            ViewData["ListaClientes"] = new SelectList(_context.Users.Where(e => e.Eliminado == false), "Id", "PrimeiroNome");

            double NrDias = 0;
            double NrHoras = 0;
            double NrMinutos = 0;
            if (pedido.VeiculoId == null)
            {
                ModelState.AddModelError("VeiculoId", "Id Veiculo null");
            }
            if (pedido.DataLevantamento > pedido.DataEntrega)
                ModelState.AddModelError("DataLevantamento", "A data de levantamento não pode ser posterior à data de entrega");
            if (pedido.DataLevantamento.Date == pedido.DataEntrega.Date)
                ModelState.AddModelError("DataLevantamento", "Tem de alugar o veículo por pelo menos 1 dia");

            var veiculo = _context.Veiculo.Find(pedido.VeiculoId);
            if (veiculo == null)
            {
                ModelState.AddModelError("VeiculoId", "Veiculo inválido");
            }

            if (ModelState.IsValid)
            {
                NrDias = (pedido.DataEntrega - pedido.DataLevantamento).TotalDays;
                NrHoras = (pedido.DataEntrega - pedido.DataLevantamento).TotalHours;
                NrMinutos = (pedido.DataEntrega - pedido.DataLevantamento).TotalMinutes;

                Reserva x = new Reserva();
                x.DataLevantamento = pedido.DataLevantamento;
                x.DataEntrega = pedido.DataEntrega;
                x.DuracaoEmHoras = NrHoras;
                x.DuracaoEmMinutos = NrMinutos;
                x.VeiculoId = (int)pedido.VeiculoId;

                x.Custo = Math.Round(veiculo.Preco * NrDias);
                x.veiculo = veiculo;
                x.ClienteId = _userManager.GetUserId(User);
                x.cliente = _context.Users.Find(x.ClienteId);

                return View("PedidoConfirmacao", x);

            }

            return View("pedido", pedido);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> AsMinhasReservas()
        {
            var reservas = _context.Reserva.Include(a => a.veiculo).Include(a => a.cliente).Where(a => a.ClienteId == _userManager.GetUserId(User) && a.Eliminado == false);
            return View(await reservas.ToListAsync());
        }

        public static bool VeiculoDisponivel(Veiculo veiculo, DateTime dataLevantamento, DateTime dataEntrega)
        {
            if (veiculo.reservas != null)
            {
                foreach (var reserva in veiculo.reservas)
                {
                    if (!(dataEntrega.CompareTo(reserva.DataLevantamento) < 0 || dataLevantamento.CompareTo(reserva.DataEntrega) > 0) && reserva.Eliminado == false && reserva.Rejeitada == false)
                        return false;
                }
            }
            return true;
        }
    }
}
