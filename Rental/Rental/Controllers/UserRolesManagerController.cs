using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Rental.Data;
using Rental.Models;
using Rental.ViewModels;
using System.Data;

namespace Rental.Controllers
{
    [Authorize(Roles = "Gestor,Admin")]
    public class UserRolesManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public UserRolesManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var UserAtual = await _userManager.GetUserAsync(User);
            if (UserAtual == null)
                return NotFound();
            var UserAtualRoles = await _userManager.GetRolesAsync(UserAtual);
            List<UserRolesViewModel> userRolesViewModels = new List<UserRolesViewModel>();
            if (UserAtual != null && UserAtualRoles != null)
            {
                if (UserAtualRoles.Contains("Gestor")) //Tem apenas acesso aos utilizadores da sua empresa
                {
                    if (UserAtual.EmpresaId != null)
                    {
                        var users = await _userManager.Users.Where(u => u.EmpresaId == UserAtual.EmpresaId && u.Eliminado == false).ToListAsync();
                        foreach (var u in users)
                        {
                            UserRolesViewModel userRolesView = new UserRolesViewModel();
                            userRolesView.UserId = u.Id;
                            userRolesView.UserName = u.UserName;
                            userRolesView.PrimeiroNome = u.PrimeiroNome;
                            userRolesView.UltimoNome = u.UltimoNome;
                            userRolesView.Avatar = u.Avatar;
                            userRolesView.Roles = await _userManager.GetRolesAsync(u);
                            userRolesView.Ativo = u.Ativo;
                            userRolesView.DataRegisto = u.DataRegisto;
                            userRolesViewModels.Add(userRolesView);
                        }
                        return View(userRolesViewModels);
                    }
                }
                else if (UserAtualRoles.Contains("Admin"))
                {
                    var users = await _userManager.Users.Where(e => e.Eliminado == false).ToListAsync();
                    foreach (var u in users)
                    {
                        UserRolesViewModel userRolesView = new UserRolesViewModel();
                        userRolesView.UserId = u.Id;
                        userRolesView.UserName = u.UserName;
                        userRolesView.PrimeiroNome = u.PrimeiroNome;
                        userRolesView.UltimoNome = u.UltimoNome;
                        userRolesView.Avatar = u.Avatar;
                        userRolesView.Roles = await _userManager.GetRolesAsync(u);
                        userRolesView.Ativo = u.Ativo;
                        userRolesView.DataRegisto = u.DataRegisto;
                        userRolesViewModels.Add(userRolesView);
                    }
                    return View(userRolesViewModels);
                }
            }
            return View();
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        //GET
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("PrimeiroNome,UltimoNome,DataNascimento,NIF,Avatar,AvatarFile,EmpresaId,roleEmpresa, Eliminado, DataRegisto,Ativo")] ApplicationUser user)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (user.AvatarFile != null)
            {
                if (user.AvatarFile.Length > 900000)
                {
                    ModelState.AddModelError("", "Erro: Ficheiro demasiado grande");
                    return View(user);
                }               
                if (!IsValidFileType(user.AvatarFile.FileName))
                {
                    ModelState.AddModelError("", "Error: Ficheiro não suportado");
                    return View(user);
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var User = await _context.Users.Where(i => i.Id == id && i.Eliminado == false).FirstOrDefaultAsync();
                    if (user.AvatarFile != User.AvatarFile)
                    {
                        using (var dataStream = new MemoryStream())
                        {
                            await user.AvatarFile.CopyToAsync(dataStream);
                            user.Avatar = dataStream.ToArray();
                        }
                        User.AvatarFile = user.AvatarFile;
                        User.Avatar = user.Avatar;
                    }
                    if (User != null)
                    {
                        if (user.PrimeiroNome != User.PrimeiroNome)
                            User.PrimeiroNome = user.PrimeiroNome;
                        if (user.UltimoNome != User.UltimoNome)
                            User.UltimoNome = user.UltimoNome;
                        if (user.DataNascimento != User.DataNascimento)          
                            User.DataNascimento = user.DataNascimento;
                        if (user.DataRegisto != User.DataRegisto)
                            User.DataRegisto = user.DataRegisto;
                        if (user.NIF != User.NIF)
                            User.NIF = user.NIF;
                        if (user.Ativo != User.Ativo)
                            User.Ativo = user.Ativo;
                        await _userManager.UpdateAsync(User);
                        if (User.Ativo == false)
                        {
                            User.LockoutEnabled = true;
                            User.LockoutEnd = DateTime.Now.AddYears(100);
                            await _userManager.UpdateAsync(User);
                        }  
                        else if(User.Ativo == true)
                        {
                            User.LockoutEnabled = true;
                            User.LockoutEnd = DateTime.Now;
                            await _userManager.UpdateAsync(User);
                        }
                        await _context.SaveChangesAsync();
                    }
                    else
                        return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        public async Task<IActionResult> Details(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                var roles = await _roleManager.Roles.ToListAsync();

                ViewBag.UserName = user.UserName;
                ViewBag.Avatar = user.Avatar;
                List<ManageUserRolesViewModel> manageUserRolesViewModels = new List<ManageUserRolesViewModel>();
                foreach (var r in roles)
                {
                    ManageUserRolesViewModel manageUserRolesViewModel = new ManageUserRolesViewModel();
                    manageUserRolesViewModel.RoleId = r.Id;
                    manageUserRolesViewModel.RoleName = r.Name;
                    if (await _userManager.IsInRoleAsync(user, r.Name))
                        manageUserRolesViewModel.Selected = true;
                    else
                        manageUserRolesViewModel.Selected = false;
                    manageUserRolesViewModels.Add(manageUserRolesViewModel);
                }
                return View(manageUserRolesViewModels);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Details(List<ManageUserRolesViewModel> model,
        string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove from existing role");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(u => u.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add role");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        //Get
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id) || await _userManager.FindByIdAsync(id) != null)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var UserRoles = await _userManager.GetRolesAsync(user);
                    if (_userManager.GetUserId(User) == user.Id)  //não pode apagar a si próprio
                    {
                        ModelState.AddModelError("", "Não é possivel eliminar o próprio");
                    }
                    else if (UserRoles != null && UserRoles.Contains("Funcionario") && user.Reservas != null) //se for funcionário, não pode apagar se estiver com reservas ativas, percorrer a Icollection de reservas do user, e verificar se o atributo da reserva Terminado está a true e o EmTransito a false  
                    {
                        foreach (var reserva in user.Reservas)
                            if (!reserva.Terminado)
                                ModelState.AddModelError("", "Não é possivel eliminar o funcionário pois este tem reservas ativas");
                    }
                    else
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (!result.Succeeded)
                            ModelState.AddModelError("", "Não foi possivel eliminar o utilizador");
                    }
                }
                else
                    return NotFound();
            }
            else
                return NotFound();
            return RedirectToAction("Index");
        }

        //GET
        public async Task<IActionResult> AdicionarUser()
        {
            UserRolesViewModel userView = new();
            var UserAtual = await _userManager.GetUserAsync(User);
            if (UserAtual == null)
                return NotFound();
            var UserAtualRoles = await _userManager.GetRolesAsync(UserAtual);
            if (UserAtualRoles.Contains("Gestor")) //Pode apenas criar funcionarios ou gestores associados à sua empresa
            {
                ViewBag.Roles = new SelectList(_roleManager.Roles.Where(r => r.Name.Equals("Gestor") || r.Name.Equals("Funcionario")).ToList(), "Name");
            }
            if (UserAtualRoles.Contains("Admin"))
            {
                ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name");
                ViewBag.EmpresaId = new SelectList(_context.Empresa.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
            }
            return View(userView);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarUser([Bind("UserId", "Email", "Password", "PrimeiroNome", "UltimoNome", "UserName", "Avatar", "Roles", "RoleId", "EmpresaId")] UserRolesViewModel userViewModel)
        {
            var UserAtual = await _userManager.GetUserAsync(User);
            if (UserAtual == null)
                return NotFound();
            var UserAtualRoles = await _userManager.GetRolesAsync(UserAtual);
            if (UserAtualRoles.Contains("Gestor")) //Pode apenas criar funcionarios ou gestores associados à sua empresa
            {
                ViewBag.Roles = new SelectList(_roleManager.Roles.Where(r => r.Name.Equals("Gestor") || r.Name.Equals("Funcionario")).ToList(), "Name");
                if (UserAtual.EmpresaId != null)
                {
                    var empresa = await _context.Empresa.FindAsync(UserAtual.EmpresaId);
                    if (empresa == null)
                        return NotFound();
                    else
                    {
                        ApplicationUser user = new()
                        {
                            UserName = userViewModel.Email,
                            Email = userViewModel.Email,
                            PrimeiroNome = userViewModel.PrimeiroNome,
                            UltimoNome = userViewModel.UltimoNome,
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = true,
                            empresa = UserAtual.empresa,
                            EmpresaId = UserAtual.EmpresaId,
                            roleEmpresa = userViewModel.RoleId,
                            DataRegisto = DateTime.Now
                        };
                        ApplicationUser user1 = await _userManager.FindByEmailAsync(user.Email);
                        if (user1 == null)
                        {
                            var result = await _userManager.CreateAsync(user, userViewModel.Password);
                            if (userViewModel.RoleId != null)
                                await _userManager.AddToRoleAsync(user, userViewModel.RoleId);
                            else
                            {
                                ModelState.AddModelError("", "Não foi possivel adicionar  o role!");
                            }
                            if (result.Succeeded)
                            {
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                foreach (IdentityError error in result.Errors)
                                    ModelState.AddModelError("", error.Description);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Esse user já existe!");
                        }
                    }

                }
            }
            else if (UserAtualRoles.Contains("Admin"))
            {
                ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name");
                ViewBag.EmpresaId = new SelectList(_context.Empresa.Where(e => e.Eliminado == false).ToList(), "Id", "Nome");
                ApplicationUser user = new()
                {
                    UserName = userViewModel.Email,
                    Email = userViewModel.Email,
                    PrimeiroNome = userViewModel.PrimeiroNome,
                    UltimoNome = userViewModel.UltimoNome,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    DataRegisto = DateTime.Now
                };
                if (userViewModel.EmpresaId != null && userViewModel.RoleId != null && (userViewModel.RoleId.Equals("Gestor") || userViewModel.RoleId.Equals("Funcionario")))
                {
                    user.EmpresaId = userViewModel.EmpresaId;
                    user.empresa = _context.Empresa.Find(userViewModel.EmpresaId);
                }
                ApplicationUser user1 = await _userManager.FindByEmailAsync(user.Email);
                if (user1 == null)
                {
                    var result = await _userManager.CreateAsync(user, userViewModel.Password);
                    if (userViewModel.RoleId != null)
                        await _userManager.AddToRoleAsync(user, userViewModel.RoleId);
                    if (userViewModel.RoleId.Equals("Gestor") || userViewModel.RoleId.Equals("Funcionario"))
                    {
                        user.roleEmpresa = userViewModel.RoleId;
                        _context.SaveChanges();
                    }
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
                    ModelState.AddModelError("", "Esse user já existe!");
                }
            }
            return View();
        }

        // GET:Grafico Clientes Novos
        public IActionResult GraficoNovosClientes()
        {
            return View();
        }

        [HttpPost]
        // POST: Cursos/GraficoVendas/5
        public async Task<IActionResult> GetDadosNovosClientes()
        {
            List<object> dados = new List<object>();
            List<ApplicationUser> users = await _userManager.Users.Where(u => u.Eliminado == false).ToListAsync();
            List<ApplicationUser> clientes = new();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Cliente"))
                    if (Math.Abs((user.DataRegisto.Value.Month - DateTime.Now.Month) + 12 * (user.DataRegisto.Value.Year - DateTime.Now.Year)) <= 12)
                        clientes.Add(user);
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("Mês", System.Type.GetType("System.String"));
            dt.Columns.Add("Clientes", System.Type.GetType("System.Int32"));
            DataRow dr = dt.NewRow();
            dr["Mês"] = "Janeiro";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 1).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Fevereiro";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 2).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Março";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 3).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Abril";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 4).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Maio";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 5).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Junho";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 6).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Julho";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 7).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Agosto";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 8).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Setembro";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 9).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Outubro";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 10).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Novembro";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 11).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Mês"] = "Dezembro";
            dr["Clientes"] = clientes.Where(d => d.DataRegisto.Value.Month == 12).Count();
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }
            return Json(dados);
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

    private bool UserExists(string id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
}

