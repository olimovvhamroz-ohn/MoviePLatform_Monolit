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
        CreateMap<MovieRequest, MovieEntity>();
        CreateMap<MovieEntity, MovieResponse>();
        CreateMap<CategoryRequest, CategoryEntity>();
        CreateMap<CategoryEntity, CategoryResponse>();
        CreateMap<ReviewRequest, ReviewEntity>();
        CreateMap<ReviewEntity, ReviewResponse>();
        CreateMap<ActorRequest, ActorEntity>();
        CreateMap<ActorEntity, ActorResponse>();
        CreateMap<StudioRequest, StudioEntity>();
        CreateMap<StudioEntity, StudioResponse>();
        CreateMap<UserEntity, UserResponse>();
        CreateMap<CartEntity, CartResponse>();
        CreateMap<PurchaseRequest, PurchaseEntity>();
        CreateMap<PurchaseEntity, PurchaseResponse>();
        CreateMap<WatchlistRequest, WatchlistEntity>();
        CreateMap<WatchlistEntity, WatchlistResponse>();
    }
}