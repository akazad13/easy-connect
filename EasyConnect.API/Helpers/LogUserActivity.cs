using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyConnect.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace EasyConnect.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            var resultContext = await next();

            var userId = int.Parse(
                resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
            );
            var repo = resultContext.HttpContext.RequestServices.GetService<IMemberRepository>();
            var user = await repo.GetUser(userId, true);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
}
