using MLearning.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MLearning.Web.Controllers
{
    class MessagesActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MLController controller = filterContext.Controller as MLController;
            if (controller != null)
                controller.Toastr = (controller.TempData["Toastr"] as Toastr) ?? new Toastr();

            base.OnActionExecuting(filterContext);
        }

        
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            MLController controller = filterContext.Controller as MLController;
            if (filterContext.Result.GetType() == typeof(ViewResult))
            {
                if (controller.Toastr != null && controller.Toastr.ToastMessages.Count() > 0)
                {
                    controller.ViewData["Toastr"] = controller.Toastr;
                }
            }
            else if (filterContext.Result.GetType() == typeof(RedirectToRouteResult))
            {
                if (controller.Toastr != null && controller.Toastr.ToastMessages.Count() > 0)
                {
                    controller.TempData["Toastr"] = controller.Toastr;
                }
            }
            else if(filterContext.Result.GetType() == typeof(JsonResult))
            {
                if (controller.Toastr != null && controller.Toastr.ToastMessages.Count() > 0)
                {
                    controller.ViewData["Toastr"] = controller.Toastr;

                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
