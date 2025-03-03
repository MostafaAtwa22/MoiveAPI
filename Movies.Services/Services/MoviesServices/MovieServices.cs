namespace Movies.Services.Services.MoviesServices
{
    public class MovieServices : IMovieServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MovieServices(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<DetailsMovieDTO>> GetAll()
        {
            var MoviesDB = await _unitOfWork.Movies
                .GetAllAsync(includes: new[] { "Genera" });

            var movies = _mapper.Map<List<DetailsMovieDTO>>(MoviesDB)
                .OrderBy(m => m.Rate)
                .ToList();

            return movies;
        }

        public async Task<DetailsMovieDTO?> GetById(int id)
        {
            var MoviesDB = await _unitOfWork
                .Movies.GetByIdAsync(id, includes: new[] { "Genera" });
            var movies = _mapper.Map<DetailsMovieDTO>(MoviesDB);
            return movies;
        }

        public async Task<Movie> Create(CreateMovieDTO dto)
        {
            var validGenere = await _unitOfWork.Generas.FindAsync(g => g.Id == dto.GeneraId);

            if (validGenere is null)
                return null!;

            using var dataStream = new MemoryStream();
            dto?.Poster.CopyToAsync(dataStream);

            var movie = _mapper.Map<Movie>(dto);

            movie.Poster = dataStream.ToArray();

            await _unitOfWork.Movies.AddAsync(movie);
            await _unitOfWork.Complete();

            return movie;
        }

        public async Task<Movie> Update(int id, UpdateMovieDTO dto)
        {
            var DbMovie = await _unitOfWork.Movies.GetByIdAsync(id);

            if (DbMovie is null)
                return null!;

            var validGenere = await _unitOfWork.Generas.FindAsync(g => g.Id == dto.GeneraId);

            if (validGenere is null)
                return null!;

            _mapper.Map(dto, DbMovie);

            if (dto.Poster is not null)
            {
                using var dataStream = new MemoryStream();
                dto?.Poster.CopyToAsync(dataStream);

                DbMovie.Poster = dataStream.ToArray();
            }
            DbMovie.GeneraId = dto!.GeneraId;


            _unitOfWork.Movies.Update(DbMovie);
            await _unitOfWork.Complete();

            return DbMovie;
        }

        public async Task<Movie> Delete(int id)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id);

            if (movie is null)
                return null!;

            _unitOfWork.Movies.Delete(movie);
            await _unitOfWork.Complete();

            return movie;
        }
    }
}
