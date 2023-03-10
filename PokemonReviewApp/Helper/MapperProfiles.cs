using AutoMapper;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Helper;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Pokemon, PokemonDto>();
        CreateMap<PokemonDto, Pokemon>();

        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();

        CreateMap<Country, CountryDto>();
        CreateMap<CountryDto, Country>();

        CreateMap<Owner, OwnerDto>();
        CreateMap<OwnerDto, Owner>();

        CreateMap<Review, ReviewDto>();
        CreateMap<ReviewDto, Review>();

        CreateMap<Reviewer, ReviewerDto>();
        CreateMap<Reviewer, ReviewerWithReviewsDto>();
        CreateMap<ReviewerDto, Reviewer>();
    }
}