namespace Movies.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IBaseRepository<Genera> Generas { get; private set; }
        public IBaseRepository<Movie> Movies { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Generas = new BaseRepository<Genera>(_context);
            Movies = new BaseRepository<Movie>(_context);
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
