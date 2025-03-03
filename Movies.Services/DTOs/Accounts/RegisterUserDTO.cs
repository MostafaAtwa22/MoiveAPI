namespace Movies.Services.DTOs.Accounts
{
    public class RegisterUserDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{6,}$",
            ErrorMessage = "Password must be at least 6 characters long, contain at least one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Role { get; set; } = default!;
    }
}
