using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IUsersService
    {
        int GetUserId();
    }

    public class UsersService: IUsersService
    {
        private readonly HttpContext httpContext;

        public UsersService(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }

        public int GetUserId()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User
                    .Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("The user is not authenticated");
            }
        }
    }
}
