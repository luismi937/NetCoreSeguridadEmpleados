using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace NetCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            //necesitamos el action y el controller de donde 
            //el usuario ha pulsado 
            //para ello, tenemos RouteValues que contiene 
            //la informacion

            //RouteData["controller"] y RouteData["action"]
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            var ruta = context.RouteData.Values["id"];

            ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
            //esta clase contiene el TEMPDATA de nuestra app
            var tempData = provider.LoadTempData(context.HttpContext);
            //almacenamos la informacion
            tempData["controller"] = controller;
            tempData["action"] = action;
            //debemos preguntar por el id 
            if (ruta != null)
            {
                tempData["id"] = ruta.ToString();
            }
            else
            {
                //eliminamos la id por si acaso ya existia en el tempdata
                tempData.Remove("id");

            }
            //Reasignamos el tampdata para nuestra app
            provider.SaveTempData(context.HttpContext, tempData);






            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = GetRoute("Managed", "Login");
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

