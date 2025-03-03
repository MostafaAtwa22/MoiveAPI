namespace Movies.Services.DTOs.Movies
{
    public class DetailsMovieDTO : MovieDTO
    {
        public int Id { get; set; }
        public byte[] Poster { get; set; } = default!;
        public Genera Genera { get; set; } = default!;
    }
}
