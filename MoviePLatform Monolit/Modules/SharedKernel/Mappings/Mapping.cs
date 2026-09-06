

using AutoMapper;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<CategoryRequest, CategoryEntity>();
        CreateMap<CategoryEntity, CategoryResponse>();
        
        CreateMap<ReviewEntity,ReviewRespose>();
        CreateMap<ReviewRequest, ReviewEntity>();

        CreateMap<ActorRequest, ActorEntity>();
        CreateMap<ActorEntity, ActorResponse>();
        
        CreateMap<StudioRequest, StudioEntity>();
        CreateMap<StudioEntity, StudioResponse>();
        
        CreateMap<MovieRequest, MovieEntity>();
        CreateMap<MovieEntity, MovieResponse>();
        CreateMap<UserRequest, UserEntity>();
        CreateMap<UserEntity,UserResponse>();

        CreateMap<CartRequest, CartEntity>();
        CreateMap<CartEntity, CartResponse>();


        CreateMap<PurchaseRequest, PurchaseEntity>();
        CreateMap<PurchaseEntity, PurchaseResponse>();
        CreateMap<WatchlistRequest, WatchlistEntity>();
        CreateMap<WatchlistEntity, WatchlistResponse>();


        
    }
}