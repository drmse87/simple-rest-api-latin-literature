using Microsoft.AspNetCore.Http;
using System.Linq;

namespace latin_web_api
{
    public class JwtUserContext : IUserContextService  {
    private readonly IHttpContextAccessor _httpContext;
    private static string _userIdField = "UserId";
    public JwtUserContext(IHttpContextAccessor httpContext) 
    {
        _httpContext = httpContext;
    }
    public int? GetId()
    {
        string storedUserId = _httpContext
            .HttpContext?
            .User?
            .Claims
            .FirstOrDefault(c => c.Type == _userIdField)?
            .Value;

        return int.TryParse(storedUserId, out int i) ? (int?)i : null;
    }
}
}