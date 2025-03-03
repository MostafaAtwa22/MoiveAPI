namespace Movies.Services.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(dest => dest.Poster, opt => opt.Ignore())
                .ForMember(dest => dest.Genera, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UpdateMovieDTO, Movie>()
                .ForMember(dest => dest.Poster, opt => opt.Ignore())
                .ForMember(dest => dest.Genera, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Movie, DetailsMovieDTO>().ReverseMap();

            CreateMap<Genera, GenereDto>().ReverseMap();
        }
    }
}
