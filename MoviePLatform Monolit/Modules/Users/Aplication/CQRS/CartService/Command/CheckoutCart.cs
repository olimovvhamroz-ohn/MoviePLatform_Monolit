

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record ChekoutCartCommand(int userId):IRequest<CartResponse>;

public class CheckoutCartCommandHandler:IRequestHandler<ChekoutCartCommand, CartResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutCartCommandHandler> _logger;

    public CheckoutCartCommandHandler(ICartRepository cartRepository, ILogger<CheckoutCartCommandHandler> logger, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<CartResponse> Handle(ChekoutCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checkout cart for user {UserId}", request.userId);

        var cart = await _cartRepository.GetActiveCartByUserId(request.userId);
        if (cart == null)
            throw new NotFoundException("Cart not found");

        if (!cart.CartItems.Any())
            throw new BadRequestException("Cart is empty");

        await _cartRepository.CheckoutCart(request.userId);
        var ubdate=await _cartRepository.GetActiveCartByUserId(request.userId);
        return  _mapper.Map<CartResponse>(ubdate);
    }
    
    
    
}