using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Commerce.Aplication.SQRS.Payments;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record ChekoutCartCommand(int userId, string? paymentToken = null) : IRequest<CartResponse>;

public class CheckoutCartCommandHandler : IRequestHandler<ChekoutCartCommand, CartResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IPaymentGeteway _paymentGateway;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutCartCommandHandler> _logger;

    public CheckoutCartCommandHandler(
        ICartRepository cartRepository,
        IPaymentGeteway paymentGateway,
        ILogger<CheckoutCartCommandHandler> logger,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _paymentGateway = paymentGateway;
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

        var lines = await _cartRepository.GetPayableLines(request.userId);
        var total = lines.Sum(l => l.Price);

        // 1. Order + Payment (Pending) дар база
        var orderId = await _cartRepository.CreatePendingOrder(request.userId, lines);

        // 2. Пардохт (агар ройгон бошад, шлюз даъват намешавад)
        bool paid;
        try
        {
            paid = total <= 0 || await _paymentGateway.PayAsync(request.userId, total);
        }
        catch
        {
            await _cartRepository.MarkOrderFailed(orderId);
            throw;
        }

        if (!paid)
        {
            await _cartRepository.MarkOrderFailed(orderId);
            _logger.LogWarning("Payment failed for user {UserId}, order {OrderId}", request.userId, orderId);
            throw new BadRequestException("Payment failed");
        }

        // 3. Харид + Completed + бастани корзина
        await _cartRepository.CompleteCheckout(request.userId, orderId, lines);

        _logger.LogInformation("Order {OrderId} completed for user {UserId}, total {Total}", orderId, request.userId, total);

        var updated = await _cartRepository.GetActiveCartByUserId(request.userId);
        return _mapper.Map<CartResponse>(updated);
    }
}