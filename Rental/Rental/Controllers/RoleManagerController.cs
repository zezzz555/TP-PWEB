using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Rental.Controllers
{
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleManagerController(RoleManager<IdentityRole> roleManager)
        {

            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            IdentityRole newRole = new IdentityRole();
            newRole.Name = roleName;
            if (!String.IsNullOrEmpty(roleName))
            {
                await _roleManager.CreateAsync(newRole);
            }           
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (await _roleManager.RoleExistsAsync(id) || !string.IsNullOrEmpty(id))
            {
                IdentityRole role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                    await _roleManager.DeleteAsync(role);
            }
            else
                return NotFound();

            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!await _roleManager.RoleExistsAsync(id))
                return Problem("Role is null.");
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
                await _roleManager.DeleteAsync(role);

            return RedirectToAction(nameof(Index));
        }
    }
}
