using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NetCoreSeguridadEmpleados.Models;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private Repositories.RepositoryHospital repo;
        public ManagedController(Repositories.RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            int idEmpleado = int.Parse(password);
            Empleado empleado = await this.repo.LogInEmpleadoAsync(username, idEmpleado);
            if (empleado != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                    ClaimTypes.Role);
                Claim claimName = new Claim(ClaimTypes.Name, username);
                identity.AddClaim(claimName);
                Claim claimId = new Claim(ClaimTypes.NameIdentifier, empleado.IdEmpleado.ToString());
                identity.AddClaim(claimId);
                Claim claimRole = new Claim(ClaimTypes.Role, empleado.Oficio);
                identity.AddClaim(claimRole);
                Claim claimSalario = new Claim("Salario", empleado.Salario.ToString());
                identity.AddClaim(claimSalario);
                Claim claimDept = new Claim("Departamento", empleado.IdDepartamento.ToString());
                identity.AddClaim(claimDept);

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                return RedirectToAction("PerfilEmpleado", "Empleados");


            }
            else
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }

        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
