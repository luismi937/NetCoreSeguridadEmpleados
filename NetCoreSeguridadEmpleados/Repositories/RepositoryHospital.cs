using Microsoft.EntityFrameworkCore;
using NetCoreSeguridadEmpleados.Data;
using NetCoreSeguridadEmpleados.Models;

namespace NetCoreSeguridadEmpleados.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }
        public async Task<List<Empleado>> GetEmpleadosAsync()
        {

            return await context.Empleados.ToListAsync();
        }
        public async Task<Empleado> FindEmpleadoAsync(int idempleado)
        {
            return await context.Empleados.FirstOrDefaultAsync(x => x.IdEmpleado == idempleado);
        }
        public async Task<List<Empleado>> GetEmpleadosDepartamentoAsync(int idDepartamento)
        {
            return await context.Empleados.Where(x => x.IdDepartamento == idDepartamento).ToListAsync();
        }
        public async Task UpdateSalarioEmpleadosAsync(int incremento, int idDepartamento)
        {
            List<Empleado> empleados = await GetEmpleadosDepartamentoAsync(idDepartamento);
            foreach (Empleado emp in empleados)
            {
                emp.Salario += incremento;
            }
            await context.SaveChangesAsync();
        }
        public async Task<Empleado> LogInEmpleadoAsync(int idEmpleado, string apellido)
        {
            Empleado empleaado = await this.context.Empleados.FirstOrDefaultAsync(x => x.IdEmpleado == idEmpleado && x.Apellido == apellido);
            return empleaado;
        }
    }
}
