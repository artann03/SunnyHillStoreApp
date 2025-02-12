namespace SunnyHillStore.Model.Models.Users
{
    public class UserProfileResponseDto
    {
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime LastLoginTime { get; set; }
    }
} 