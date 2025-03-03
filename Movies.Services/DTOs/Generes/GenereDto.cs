namespace Movies.Services.DTOs.Generes
{
    public class GenereDto
    {
        [Required]
        [MaxLength(100), MinLength(3)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty;
    }
}
