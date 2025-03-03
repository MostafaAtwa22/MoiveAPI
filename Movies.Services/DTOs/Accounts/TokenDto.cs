namespace Movies.Services.DTOs.Accounts
{
    public class TokenDto
    {
        public string RefreshToke { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
