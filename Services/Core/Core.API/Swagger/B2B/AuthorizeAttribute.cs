using Core.API.Repositories.B2B;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Core.API.Swagger.B2B
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IB2BRepository b2bRepository;

        #region Constructor

        public AuthorizeAttribute(IB2BRepository repository)
        {
            this.b2bRepository = repository;
        }

        #endregion

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

                var param = b2bRepository.ValidateB2BToken(token);
                if (param == null)
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                else
                {
                    context.HttpContext.Items.Add("CompanyGuid", param.CompanyGuid);
                }
            }
        }
    }
}
