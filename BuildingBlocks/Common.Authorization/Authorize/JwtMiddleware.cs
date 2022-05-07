using Common.Authorization.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Authorization.Authorize
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Models.TokenSettings _tokenSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<Models.TokenSettings> appSettings)
        {
            _next = next;
            _tokenSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateJwtToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                //context.Items["User"] = userService.GetById(userId.Value);
                context.Items["User"] = new Common.Models.Users.User();
            }

            await _next(context);
        }
    }
}
