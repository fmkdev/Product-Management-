using Microsoft.AspNetCore.Identity;

namespace API.Domain.Entities
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
