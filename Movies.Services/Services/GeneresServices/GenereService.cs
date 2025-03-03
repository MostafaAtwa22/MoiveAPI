
namespace Movies.Services.Services.GeneresServices
{
    public class GenereService : IGenereService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenereService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Genera>> GetAll()
            => await _unitOfWork.Generas.GetAllAsync();

        public async Task<Genera?> GetById(int id)
            => await _unitOfWork.Generas.GetByIdAsync(id);

        public async Task Create(Genera g)
        {
            await _unitOfWork.Generas.AddAsync(g);
            await _unitOfWork.Complete();
        }

        public async Task<Genera> Update(int id, GenereDto dto)
        {
            var genere = await _unitOfWork.Generas.GetByIdAsync(id);

            if (genere is null)
                return null!;

            _mapper.Map(dto, genere);

            var updated = _unitOfWork.Generas.Update(genere);

            if (updated is null)
                return null!;

            await _unitOfWork.Complete();

            return genere;
        }

        public async Task<Genera> Delete(int id)
        {
            var genere = await _unitOfWork.Generas.GetByIdAsync(id);

            if (genere is null)
                return null!;

            _unitOfWork.Generas.Delete(genere);

            await _unitOfWork.Complete();

            return genere;
        }
    }
}
