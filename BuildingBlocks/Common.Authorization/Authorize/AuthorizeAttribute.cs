using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Common.Authorization.Authorize
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            context.HttpContext.Request.Headers.TryGetValue("Token", out var token);
            {
                if (token.Count() == 0 || token[0] == null)
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

                var jwtUtil = new JwtUtils();
                var param = jwtUtil.ValidateJwtToken(token, true, false);
                if (param == null)
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                else
                {
                    param.Token = token;
                    context.HttpContext.Items.Add("CurrentUserDetails", param);

                    // Need to remove after all changes
                    context.HttpContext.Items["UserId"] = param.UserId;
                    context.HttpContext.Items["CompanyGuid"] = param.CompanyGuid;
                    context.HttpContext.Items["UserGuid"] = param.UserGuid;
                    context.HttpContext.Items["UserTypeId"] = param.UserTypeId;
                    context.HttpContext.Items["Token"] = token;
                }
            }
        }
    }
}
