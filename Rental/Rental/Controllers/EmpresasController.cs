using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental.Data;
using Rental.Models;
using Rental.ViewModels;

namespace Rental.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EmpresasController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Empresas
        public async Task<IActionResult> Index()
        {
            ListaEmpresasFuncionariosViewModel pesquisa = new();
            ViewBag.Nome = new SelectList(_context.Empresa.Where(e => e.Eliminado == false).ToList(),"Id", "Nome");
            var emps = await _context.Empresa.Include(g => g.funcionarios).Where(e => e.Eliminado == false).ToListAsync();
            List<EmpresaFuncionariosViewmodel> ListaEmpresasFuncionarios = new();
            foreach (var emp in emps)
            {
                if (emp.funcionarios != null)
                {
                    var empresaFuncionarios = new EmpresaFuncionariosViewmodel()
                    {
                        empresa = emp,
                        gestores = emp.funcionarios.Where(f => f.roleEmpresa != null && f.roleEmpresa.Equals("Gestor") && f.Eliminado == false).ToList(),
                        funcionarios = emp.funcionarios.Where(f => f.roleEmpresa != null && f.roleEmpresa.Equals("Funcionario") && f.Eliminado == false).ToList()

                    };
                    ListaEmpresasFuncionarios.Add(empresaFuncionarios);
                }           
            }
            pesquisa.empresas = ListaEmpresasFuncionarios;
            return View(pesquisa);
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind("empresaId", "subscricao")] ListaEmpresasFuncionariosViewModel pesquisa )
        {
            ViewBag.Nome = new SelectList(_context.Empresa.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
            List<Empresa> emps = new();
            if (pesquisa.empresaId != null)
            {
                emps = await _context.Empresa.Include(g => g.funcionarios).Where(e => e.Id == pesquisa.empresaId && e.Eliminado == false).ToListAsync();
            }
            else
            {
                emps = await _context.Empresa.Include(g => g.funcionarios).Where(e => e.Eliminado == false).ToListAsync();
            } 
            if(pesquisa.subscricao != null)
            {
                List<Empresa> empresasSubscritas = await _context.Empresa.Include(g => g.funcionarios).Where(e => e.Susbcricao.Equals(pesquisa.subscricao) && e.Eliminado == false).ToListAsync();
                emps = empresasSubscritas.Intersect(emps).ToList();
            }
            List<EmpresaFuncionariosViewmodel> ListaEmpresasFuncionarios = new();
            foreach (var emp in emps)
            {
                if (emp.funcionarios != null)
                {
                    var empresaFuncionarios = new EmpresaFuncionariosViewmodel()
                    {
                        empresa = emp,
                        gestores = emp.funcionarios.Where(f => f.roleEmpresa != null && f.roleEmpresa.Equals("Gestor") && f.Eliminado == false).ToList(),
                        funcionarios = emp.funcionarios.Where(f => f.roleEmpresa != null && f.roleEmpresa.Equals("Funcionario") && f.Eliminado == false).ToList()

                    };
                    ListaEmpresasFuncionarios.Add(empresaFuncionarios);
                }
            }
            pesquisa.empresas = ListaEmpresasFuncionarios;
            return View(pesquisa);
        }
            // GET: Empresas/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa  
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // GET: Empresas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string primeiroNome, string ultimoNome, string password, [Bind("Id,Nome,Avaliacao,Susbcricao,LogoFile,Logo,Eliminado")] Empresa empresa)
        {
            ModelState.Remove(nameof(Veiculo));
            if (empresa.LogoFile != null)
            {
                if (empresa.LogoFile.Length > 900000)
                {
                    ModelState.AddModelError("", "Erro: Ficheiro demasiado grande");
                    return View(empresa);
                }
                // método a implementar – verifica se a extensão é .png,.jpg,.jpeg
                if (!IsValidFileType(empresa.LogoFile.FileName))
                {
                    ModelState.AddModelError("", "Error: Ficheiro não suportado");
                    return View(empresa);
                }
                using (var dataStream = new MemoryStream())
                {
                    await empresa.LogoFile.CopyToAsync(dataStream);
                    empresa.Logo = dataStream.ToArray();
                }
            }
            var gestor = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PrimeiroNome = primeiroNome,
                UltimoNome = ultimoNome,
                EmailConfirmed = true,
                roleEmpresa = "Gestor"
            };
            ApplicationUser Gestor = await _userManager.FindByEmailAsync(gestor.Email);
            if (Gestor == null)
            {
                var result = await _userManager.CreateAsync(gestor, password);
                await _userManager.AddToRoleAsync(gestor, Roles.Gestor.ToString());
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("email", "Esse gestor já existe!");
            }
            if (ModelState.IsValid)
            {   
                _context.Add(empresa);
                await _context.SaveChangesAsync();            
                gestor.EmpresaId = empresa.Id;
                gestor.empresa = empresa;
                empresa.funcionarios.Add(gestor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(empresa);
        }

        // GET: Empresas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }
            var empresa = await _context.Empresa.Include(c=> c.funcionarios).Where(i =>i.Id == id && i.Eliminado == false).FirstOrDefaultAsync();
            if (empresa == null)
            {
                return NotFound();
            }
            ViewBag.Logo = empresa.Logo;
            return View(empresa);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Avaliacao,Susbcricao,LogoFile,Logo,Eliminado")] Empresa empresa)
        {
            if (id != empresa.Id)
            {
                return NotFound();
            }
            if (empresa.LogoFile != null)
            {
                if (empresa.LogoFile.Length > 900000)
                {
                    ModelState.AddModelError("", "Erro: Ficheiro demasiado grande");
                    return View(empresa);
                }
                if (!IsValidFileType(empresa.LogoFile.FileName))
                {
                    ModelState.AddModelError("", "Error: Ficheiro não suportado");
                    return View(empresa);
                }       
            }         
            //ModelState.Remove(nameof(Veiculo));
            if (ModelState.IsValid)
            {
                try
                {
                    var Empresa = await _context.Empresa.Include(c => c.funcionarios).Where(i => i.Id == id && i.Eliminado == false).FirstOrDefaultAsync();
                    if(empresa.LogoFile != Empresa.LogoFile)
                    {
                        using (var dataStream = new MemoryStream())
                        {
                            await empresa.LogoFile.CopyToAsync(dataStream);
                            empresa.Logo = dataStream.ToArray();
                        }
                        Empresa.LogoFile = empresa.LogoFile;
                        Empresa.Logo = empresa.Logo;
                    }         
                    if (Empresa != null) {
                        Empresa.Nome = empresa.Nome;
                        Empresa.Avaliacao = empresa.Avaliacao;
                        Empresa.Susbcricao = empresa.Susbcricao;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaExists(empresa.Id))
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
            return View(empresa);
        }

        // GET: Empresas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Empresa == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Empresa == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Empresa'  is null.");
            }
            var empresa = await _context.Empresa.FindAsync(id);
            if (empresa != null && (empresa.veiculos == null || empresa.veiculos.Count == 0))
            {
                //_context.Empresa.Remove(empresa);
                empresa.Eliminado = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaExists(int id)
        {
          return _context.Empresa.Any(e => e.Id == id);
        }

        // verifica se a extensão é .png,.jpg,.jpeg
        static bool IsValidFileType(string fileName)
        {
            if (fileName != null)
            {
                var extensao = fileName.Split(".");
                if (extensao[1].Contains("png") || extensao[1].Contains("jpg") || extensao[1].Contains("jpeg"))
                {                                                      
                    return true;
                }
            }
            return false;
        }
    }
}
