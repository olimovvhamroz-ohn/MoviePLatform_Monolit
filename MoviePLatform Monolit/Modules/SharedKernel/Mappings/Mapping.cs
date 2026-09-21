

using AutoMapper;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<MovieEntity, MovieResponse>();
        CreateMap<MovieRequest, MovieEntity>();
        CreateMap<CategoryRequest, CategoryEntity>();
        CreateMap<CategoryEntity, CategoryResponse>();
        
        CreateMap<ReviewEntity,ReviewResponse>();
        CreateMap<ReviewRequest, ReviewEntity>();
//CreateMap<ReviewResponse, ReviewEntity>();

        CreateMap<ActorRequest, ActorEntity>();
        CreateMap<ActorEntity, ActorResponse>();
        
        CreateMap<StudioRequest, StudioEntity>();
        CreateMap<StudioEntity, StudioResponse>();
        
        CreateMap<MovieRequest, MovieEntity>();
        CreateMap<MovieEntity, MovieResponse>();
        CreateMap<UserRequest, UserEntity>();
        CreateMap<UserEntity,UserResponse>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))      // ✅ Добавить
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone)); 

        CreateMap<CartRequest, CartEntity>();
        CreateMap<CartEntity, CartResponse>();


        CreateMap<PurchaseRequest, PurchaseEntity>();
        CreateMap<PurchaseEntity, PurchaseResponse>();
        CreateMap<WatchlistRequest, WatchlistEntity>();
        CreateMap<WatchlistEntity, WatchlistResponse>();


        
    }
}