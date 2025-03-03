namespace Movies.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Genera> Generas { get; }
        IBaseRepository<Movie> Movies { get; }

        Task<int> Complete();
    }
}
