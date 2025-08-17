using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VolunteerManagementSystem.Filters
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isAdmin = context.HttpContext.Session.GetString("Admin") == "true";
            if (!isAdmin)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
