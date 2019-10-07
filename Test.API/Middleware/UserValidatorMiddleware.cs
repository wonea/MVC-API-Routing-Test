using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Test.API.Middleware
{
    /// <summary>
    /// User Validators
    /// Validate passed user (header) authentication
    /// </summary>
    public class UserValidatorMiddleware
    {
        private readonly RequestDelegate _next;

        public UserValidatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            int x = 1 + 2;
            if (x > 1)
                await _next.Invoke(context);
        }
    }
}