using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SunnyHillStore.Core.Services.CurrentUser
{
    public class CurrentUserHelper : ICurrentUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        public string UserRole => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Anonymous";
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
