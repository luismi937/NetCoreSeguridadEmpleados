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
        [AuthorizeEmpleados]
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
        [AuthorizeEmpleados]
        public async Task<IActionResult> Compis()
        {
            string dato = HttpContext.User.FindFirstValue("Departamento");
            int idDepartamento = int.Parse(dato);
            List<Empleado> empleados = await repo.GetEmpleadosDepartamentoAsync(idDepartamento);
            return View(empleados);
        }
        [AuthorizeEmpleados(Policy = "EmpleadoPolicy")]
        [HttpPost]
        public async Task<IActionResult> Compis(int incremento)
        {
            string dato = HttpContext.User.FindFirstValue("Departamento");
            int idDept = int.Parse(dato);
            await repo.UpdateSalarioEmpleadosAsync(idDept, incremento);
            List<Empleado> empleados = await repo.GetEmpleadosDepartamentoAsync(idDept);
            await repo.GetEmpleadosDepartamentoAsync(idDept);
            return View(empleados);
        }
        [AuthorizeEmpleados(Policy = "AdminPolicy")]
        public IActionResult AdminEmpleados()
        {
            return View();
        }
        [AuthorizeEmpleados(Policy = "SalarioPolicy")]
        public IActionResult ZonaNoble()
        {
            return View();
        }

    }
}
