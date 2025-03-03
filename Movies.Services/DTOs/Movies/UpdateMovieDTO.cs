namespace Movies.Services.DTOs.Movies
{
    public class UpdateMovieDTO : MovieDTO
    {
        [AllowedExtensions(FileSettings.AllowedExtensions),
            MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile? Poster { get; set; } = default!;
    }
}
