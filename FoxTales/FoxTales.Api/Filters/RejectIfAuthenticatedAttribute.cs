using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace FoxTales.Api.Filters;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RejectIfAuthenticatedAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var authHeader = context.HttpContext.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    context.Result = new BadRequestObjectResult("You are already authenticated.");
                    return;
                }
            }
            catch
            {
            }
        }

        base.OnActionExecuting(context);
    }

}
