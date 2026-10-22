using AutoMapper;
using MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.Request;
using MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.Response;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.REQUEST;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Command.Mappings;

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
        CreateMap<WatchlistRequest, WatchlistEntity>();
        CreateMap<WatchlistEntity, WatchlistResponse>();
    }
}