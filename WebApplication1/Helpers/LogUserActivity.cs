using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication1.Extentions;
using WebApplication1.Interfaces;

namespace WebApplication1.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userid = resultContext.HttpContext.User.GetUserId();
            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userid);

            user.LastActive = DateTime.UtcNow;

            await repo.saveAllAsyns();
        }
    }
}
