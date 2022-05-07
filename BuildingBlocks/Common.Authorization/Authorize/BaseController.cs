using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace Common.Authorization.Authorize
{
    public class BaseController : ControllerBase
    {
        private TokenParams currentUserDetails = new TokenParams();
        public TokenParams CurrentUserDetails { get { return currentUserDetails; } }

        public BaseController()
        {
            if (HttpContext != null)
            {
                currentUserDetails = (TokenParams)HttpContext.Items["CurrentUserDetails"];
            }
        }
    }
}
