using Microsoft.AspNetCore.Mvc;
using NetCoreSeguridadEmpleados.Filters;
using NetCoreSeguridadEmpleados.Models;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private Repositories.RepositoryHospital repo;

        public EmpleadosController(Repositories.RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> Details(int id)
        {
            Empleado emp = await repo.FindEmpleadoAsync(id);
            return View(emp);
        }

        [AuthorizeEmpleados]
        public IActionResult PerfilEmpleado()
        {
            return View();
        }
        public async Task<IActionResult> Compis()
        {
            string dato = HttpContext.User.FindFirstValue("Departamento");
            int idDepartamento = int.Parse(dato);
            List<Empleado> empleados = await repo.GetEmpleadosDepartamentoAsync(idDepartamento);
            return View(empleados);
        }

    }
}
