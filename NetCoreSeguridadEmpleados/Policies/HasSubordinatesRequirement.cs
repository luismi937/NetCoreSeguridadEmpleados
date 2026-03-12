using Microsoft.AspNetCore.Authorization;
using NetCoreSeguridadEmpleados.Repositories;

namespace NetCoreSeguridadEmpleados.Policies
{
    public class HasSubordinatesRequirement : AuthorizationHandler<HasSubordinatesRequirement>, IAuthorizationRequirement
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasSubordinatesRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier) == false)
            {
                context.Fail();
                return;
            }

            var httpContext = context.Resource as Microsoft.AspNetCore.Http.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            var repo = httpContext.RequestServices.GetService<RepositoryHospital>();
            if (repo == null)
            {
                context.Fail();
                return;
            }

            string idEmpleadoStr = context.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idEmpleadoStr, out int idEmpleado))
            {
                bool tieneSubordinados = await repo.TieneSubordinadosAsync(idEmpleado);
                if (tieneSubordinados)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
        }
    }
}
