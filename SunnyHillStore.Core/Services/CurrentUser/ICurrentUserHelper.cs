namespace SunnyHillStore.Core.Services.CurrentUser
{
    public interface ICurrentUserHelper
    {
        string UserId { get; }
        string UserRole { get; }
        bool IsAuthenticated { get; }
    }
}