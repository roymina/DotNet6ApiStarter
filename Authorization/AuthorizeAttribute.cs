using ApiStarter.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ApiStarter.Authorization
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class AuthorizedAttribute : Attribute, IAuthorizationFilter
  {
    private readonly IList<Role> roles;
    public AuthorizedAttribute(params Role[] roles)
    {
      this.roles = roles;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
      // skip authorization if action is decorated with [AllowAnonymous] attribute
      var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
      if (allowAnonymous) return;

      var user = context.HttpContext.Items["User"] as User;
      // 用户未登录或者不符合role的要求
      if (user == null || (roles.Any() && !roles.Contains(user.Role)))
      {
        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
      }
    }
  }
}