using AutoMapper;
using Gauniv.WebServer.Data;

namespace Gauniv.WebServer.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 📌 Mapping de Game -> GameDto (lecture des jeux)
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Name).ToList()));

            // 📌 Mapping de GameDto -> Game (ajout/modification de jeux)
            CreateMap<GameDto, Game>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore l'ID car auto-généré par la BDD
                .ForMember(dest => dest.Categories, opt => opt.Ignore()); // Ignore les catégories ici

            // 📌 Mapping de Category -> CategoryDto (lecture des catégories)
            CreateMap<Category, CategoryDto>();

            // 📌 Mapping de CategoryDto -> Category (ajout/modification de catégories)
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore l'ID car auto-généré
                .ForMember(dest => dest.Games, opt => opt.Ignore()); // Ignore les jeux ici
        }
    }
}
