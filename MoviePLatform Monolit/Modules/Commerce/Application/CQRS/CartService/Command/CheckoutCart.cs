using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.CartService.Command;

public record CheckoutCartCommand(long UserId, string? PaymentToken = null) : IRequest<CartResponse>;

public class CheckoutCartCommandHandler(
    ICartRepository cartRepository,
    IPaymentGateway paymentGateway,
    ILogger<CheckoutCartCommandHandler> logger,
    IMapper mapper)
    : IRequestHandler<CheckoutCartCommand, CartResponse>
{
    public async Task<CartResponse> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checkout cart for user {UserId}", request.UserId);

        var cart = await cartRepository.GetActiveCartByUserId(request.UserId);
        if (cart == null)
            throw new NotFoundException("Cart not found");

        if (!cart.CartItems.Any())
            throw new BadRequestException("Cart is empty");

        var lines = await cartRepository.GetPayableLines(request.UserId);
        var total = lines.Sum(l => l.Price);

       
        long orderId;
        try
        {
            orderId = await cartRepository.CreatePendingOrder(request.UserId, lines);
        }
        catch (DbUpdateException)
        {
            throw new BadRequestException("Checkout already in progress");
        }

        bool paid;
        try
        {
            paid = total <= 0 || await paymentGateway.PayAsync(request.UserId, total);
        }
        catch
        {
            await cartRepository.MarkOrderFailed(orderId);
            throw;
        }

        if (!paid)
        {
            await cartRepository.MarkOrderFailed(orderId);
            logger.LogWarning("Payment failed for user {UserId}, order {OrderId}", request.UserId, orderId);
            throw new BadRequestException("Payment failed");
        }

        
        await cartRepository.CompleteCheckout(request.UserId, orderId, lines);

        logger.LogInformation("Order {OrderId} completed for user {UserId}, total {Total}", orderId, request.UserId, total);

        var updated = await cartRepository.GetActiveCartByUserId(request.UserId);
        return mapper.Map<CartResponse>(updated);
    }
}