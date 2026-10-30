using AutoMapper;
using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Request;
using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Response;
using MoviePlatformMonolit.Modules.Commerce.Domain.Entity;
using MoviePlatformMonolit.Modules.Engagement.Domain.Dto.REQUEST;
using MoviePlatformMonolit.Modules.Engagement.Domain.Dto.RESPONSE;
using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Users.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Command.Mappings;

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
        CreateMap<CartItemEntity, CartItemResponse>();
        CreateMap<PurchaseRequest, PurchaseEntity>();
        CreateMap<PurchaseEntity, PurchaseResponse>();
        CreateMap<WatchlistRequest, WatchListEntity>();
        CreateMap<WatchListEntity, WatchlistResponse>();
    }
}