namespace Movies.Services.Services.MoviesServices
{
    public interface IMovieServices
    {
        Task<List<DetailsMovieDTO>> GetAll();
        Task<DetailsMovieDTO?> GetById(int id);
        Task<Movie> Create(CreateMovieDTO dto);
        Task<Movie> Update(int id, UpdateMovieDTO dto);
        Task<Movie> Delete(int id);
    }
}
