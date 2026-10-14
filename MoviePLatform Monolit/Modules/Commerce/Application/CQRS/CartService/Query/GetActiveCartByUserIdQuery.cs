using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.CartService.Query;

public record GetActiveCartByUserId(long UserId) : IRequest<CartResponse>;
public class GetActiveCartByUserIdQuery(ICartRepository cartRepository,IMapper mapper)
    :IRequestHandler<GetActiveCartByUserId,CartResponse>
{

    public async Task<CartResponse> Handle(GetActiveCartByUserId request, CancellationToken cancellationToken)
    {
        var res = await cartRepository.GetActiveCartByUserId(request.UserId);
        
        return mapper.Map<CartResponse>(res);
    }
}