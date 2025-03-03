namespace Movies.Services.Services.GeneresServices
{
    public interface IGenereService
    {
        Task<IEnumerable<Genera>> GetAll();
        Task<Genera?> GetById(int id);
        Task Create(Genera g);
        Task<Genera> Update(int id, GenereDto dto);
        Task<Genera> Delete(int id);
    }
}
