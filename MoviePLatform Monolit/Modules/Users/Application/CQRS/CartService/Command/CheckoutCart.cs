using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.Payments;
using MoviePLatform_Monolit.Modules.SharedKernel.Interfaces;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record CheckoutCartCommand(int userId, string? paymentToken = null) : IRequest<CartResponse>;

public class CheckoutCartCommandHandler : IRequestHandler<CheckoutCartCommand, CartResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutCartCommandHandler> _logger;

    public CheckoutCartCommandHandler(
        ICartRepository cartRepository,
        IPaymentGateway paymentGateway,
        ILogger<CheckoutCartCommandHandler> logger,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _paymentGateway = paymentGateway;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<CartResponse> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checkout cart for user {UserId}", request.userId);

        var cart = await _cartRepository.GetActiveCartByUserId(request.userId);
        if (cart == null)
            throw new NotFoundException("Cart not found");

        if (!cart.CartItems.Any())
            throw new BadRequestException("Cart is empty");

        var lines = await _cartRepository.GetPayableLines(request.userId);
        var total = lines.Sum(l => l.Price);

       
        long orderId;
        try
        {
            orderId = await _cartRepository.CreatePendingOrder(request.userId, lines);
        }
        catch (DbUpdateException)
        {
            throw new BadRequestException("Checkout already in progress");
        }

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