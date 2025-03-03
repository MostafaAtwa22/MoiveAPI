using Microsoft.AspNetCore.Identity;

namespace Movies.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpirationTime { get; set; } = DateTime.Now;
    }
}
