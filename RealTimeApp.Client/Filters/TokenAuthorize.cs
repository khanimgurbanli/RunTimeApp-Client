using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class TokenAuthorize : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new RedirectToActionResult("SignIn", "Account", new { message = "Your session has expired. Please sign in again." });
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (roleClaim == null)
        {
            context.Result = new RedirectToActionResult("SignIn", "Account", new { message = "Invalid role or unauthorized access." });
            return;
        }

        // Optionally, you can store role information in TempData for use in controllers/views
        context.HttpContext.Items["UserRole"] = roleClaim;
    }
}
