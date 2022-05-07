using Common.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using static Common.Models.Enums;

namespace Common.Authorization.Authorize
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleBasedAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _userTypes;
        private readonly int[] _userRoles;

        #region Constructor

        public RoleBasedAuthorizationAttribute(int[] userTypes, params int[] userRoles)
        {
            _userTypes = userTypes;
            _userRoles = userRoles;
        }

        #endregion

        #region OnAuthorization

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("Token", out var token);
            {
                // Validate Jwt Token
                var jwtUtil = new JwtUtils();
                var param = jwtUtil.ValidateJwtToken(token, true, false);

                if (param != null)
                {
                    var authorizeStatus = (_userTypes.Contains((int)param.UserTypeId) &&
                                            (_userRoles.Contains((int)UserRoles.AllUserRoles) ||
                                             _userRoles.Intersect(param.UserRoles).Count() > 0))
                                        ? true
                                        : false;

                    if (!authorizeStatus)
                    {
                        context.Result = new JsonResult(new 
                        { message = CommonMessages.ResourceAccessPermission_Invalid }) 
                        { StatusCode = StatusCodes.Status405MethodNotAllowed };
                    }
                }
            }
        }

        #endregion
    }
}
