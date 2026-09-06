

using AutoMapper;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public class Mapping:Profile
{
    public Mapping()
    {
        CreateMap<UserRequest, UserEntity>();
        CreateMap<UserEntity,UserResponse>();

        CreateMap<CartRequest, CartEntity>();
        CreateMap<CartEntity, CartResponse>();

        CreateMap<CartItemEntity, CartItemResponse>()
            .ForMember(d => d.MovieSummary, opt => opt.MapFrom(s => s.Movie));

        CreateMap<MovieEntity, MovieSummaryResponse>();

        CreateMap<PurchaseRequest, PurchaseEntity>();
        CreateMap<PurchaseEntity, PurchaseResponse>();
    }
}