namespace Movies.Services.DTOs.Accounts
{
    public class ResetPasswordDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{6,}$",
            ErrorMessage = "Password must be at least 6 characters long, contain at least one uppercase letter, one lowercase letter, and one digit.")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Required]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
