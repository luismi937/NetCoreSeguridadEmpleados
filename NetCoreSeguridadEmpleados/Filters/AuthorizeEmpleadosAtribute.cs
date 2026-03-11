using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = GetRoute("Managed", "Login");
            }
            else
            {
                //comprobamos los roles 
                //tenemos en cuenta mayusculas y minusculas
                if (!user.IsInRole("PERSIDENTE") == false && !user.IsInRole("DIRECTOR") == false
                    && !user.IsInRole("ANALISTA") == false)
                {
                    context.Result = this.GetRoute("Managed", "ErrorAcceso");
                }
            }
        }
        //en elgun momento tendremos mas direcciones uqe solo 
        //a login por lo que creamos un metodo para seleccionar 
        private RedirectToRouteResult GetRoute(string controller, string action)
        {
            RouteValueDictionary ruta = new RouteValueDictionary(new
            {
                controller = controller,
                action = action
            });
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;

        }

    }
}

