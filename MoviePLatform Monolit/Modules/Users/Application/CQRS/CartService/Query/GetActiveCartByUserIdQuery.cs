

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public record GetActiveCartByUserId(int userId) : IRequest<CartResponse>;
public class GetActiveCartByUserIdQuery:IRequestHandler<GetActiveCartByUserId,CartResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    
    public GetActiveCartByUserIdQuery(ICartRepository cart,IMapper mapper)
    {
        _cartRepository = cart;
        _mapper = mapper;

    }

    public async Task<CartResponse> Handle(GetActiveCartByUserId request, CancellationToken cancellationToken)
    {
        var res = await _cartRepository.GetActiveCartByUserId(request.userId);
        return _mapper.Map<CartResponse>(res);
    }
}