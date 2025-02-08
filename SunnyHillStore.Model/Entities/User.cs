using SunnyHillStore.Model.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SunnyHillStore.Model.Entities
{
    public class User : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }

        public DateTime LastLoginTime { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
