using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CampusLearn_Web_App.Extensions;

namespace CampusLearn_Web_App.Filters
{
    public class RequireAuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Session.IsUserLoggedIn())
            {
                context.Result = new RedirectToPageResult("/LoginPage");
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    public class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public RequireRoleAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // Check if user is logged in
            if (!session.IsUserLoggedIn())
            {
                context.Result = new RedirectToPageResult("/LoginPage");
                return;
            }

            // Check if user has required role
            var userRole = session.GetCurrentUserRole();
            if (string.IsNullOrEmpty(userRole) || !_allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new ForbidResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    // Specific role filters for convenience
    public class RequireAdminAttribute : RequireRoleAttribute
    {
        public RequireAdminAttribute() : base("Admin") { }
    }

    public class RequireTutorAttribute : RequireRoleAttribute
    {
        public RequireTutorAttribute() : base("Tutor") { }
    }

    public class RequireStudentAttribute : RequireRoleAttribute
    {
        public RequireStudentAttribute() : base("Student") { }
    }

    public class RequireAdminOrTutorAttribute : RequireRoleAttribute
    {
        public RequireAdminOrTutorAttribute() : base("Admin", "Tutor") { }
    }
}
