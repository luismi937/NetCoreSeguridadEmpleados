using Microsoft.AspNetCore.Authorization;

namespace NetCoreSeguridadEmpleados.Policies
{
    public class OverSalarioRequirement : AuthorizationHandler<OverSalarioRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OverSalarioRequirement requirement)
        {
            //podriamos preguntar si existe un claim o no
            if (context.User.HasClaim(c => c.Type == "Salario") == false)
            {
                context.Fail();
            }
            else
            {
                string data = context.User.Claims.FirstOrDefault(c => c.Type == "Salario").Value;
                int salario = int.Parse(data);
                if (salario > 10000)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            return Task.CompletedTask;
        }
    }
}
