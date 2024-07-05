using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace RealTimeApp.Client.Filters
{
    public class TokenAuthorize : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Cookies["AccessToken"];

            if (string.IsNullOrEmpty(token) || TokenHelper.IsTokenExpired(token))
            {
                context.Result = new RedirectToActionResult("SignIn", "Account", null);
                return;
            }
        }
    }

    public static class TokenHelper
    {
        public static bool IsTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
    }
}

