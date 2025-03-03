namespace Movies.Services.DTOs.Movies
{
    public class MovieDTO
    {
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Range(1900, 2025, ErrorMessage = "Year must be between 1900 and 2025.")]
        public int Year { get; set; }

        [Range(1, 10, ErrorMessage = "Year must be between 1 and 10.")]
        public double Rate { get; set; }

        [MaxLength(2500)]
        public string StoreLine { get; set; } = string.Empty;

        public int GeneraId { get; set; }
    }
}
