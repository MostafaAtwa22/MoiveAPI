namespace Movies.Services.DTOs.Movies
{
    public class CreateMovieDTO : MovieDTO
    {
        [AllowedExtensions(FileSettings.AllowedExtensions),
            MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile Poster { get; set; } = default!;
    }
}
