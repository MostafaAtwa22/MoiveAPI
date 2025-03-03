namespace Movies.Services.DTOs.Accounts
{
    public class UserDetailsDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string[] Roles { get; set; } = default!;
    }
}
