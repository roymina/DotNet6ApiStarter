using ApiStarter.Services;
using ApiStarter.Utils;
using Microsoft.Extensions.Options;

namespace ApiStarter.Authorization
{
  public class JwtMiddleware
  {
    private readonly RequestDelegate next;
    private readonly AppSettings appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
      this.next = next;
      this.appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtService jwtService)
    {
      string token = string.Empty;
      var cookieToken = context.Request.Cookies["token"];
      if (cookieToken != null)
      {
        token = cookieToken!;
      }

      else
      {
        var headerToken = context.Request.Headers["Authorization"].FirstOrDefault();
        if (headerToken != null)
        {
          if (headerToken.Contains("Bearer "))
          {
            token = headerToken.Split(" ").Last();
          }
          else
          {
            token = headerToken;
          }
        }
      }

      var userId = jwtService.ValidateJwtToken(token);
      if (userId != null)
      {
        // attach user to context on successful jwt validation
        context.Items["User"] = userService.GetById(userId.Value);
      }



      await next(context);
    }
  }
}