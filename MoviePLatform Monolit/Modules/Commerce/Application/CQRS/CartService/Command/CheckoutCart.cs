using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;
using MoviePLatform_Monolit.Modules.Commerce.Domain.Enum;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.CartService.Command;

public record CheckoutCartCommand(long UserId, string? PaymentToken = null) : IRequest<CartResponse>;

public class CheckoutCartCommandHandler(
    ApplicationDbContext context,
    IPaymentGateway paymentGateway,
    ILogger<CheckoutCartCommandHandler> logger,
    IMapper mapper) : IRequestHandler<CheckoutCartCommand, CartResponse>
{
    public async Task<CartResponse> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checkout cart for user {UserId}", request.UserId);

        var cart = await context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && !x.IsCheckedOut, cancellationToken)
            ?? throw new NotFoundException("Cart not found");

        if (cart.CartItems.Count == 0)
            throw new BadRequestException("Cart is empty");

        var movieIds = cart.CartItems.Select(i => i.MovieId).Distinct().ToList();

        var alreadyBought = await context.Purchases
            .Where(x => x.UserId == request.UserId && movieIds.Contains(x.MovieId))
            .Select(x => x.MovieId)
            .ToListAsync(cancellationToken);

        var lines = await context.Movies
            .AsNoTracking()
            .Where(m => movieIds.Contains(m.Id) && !alreadyBought.Contains(m.Id))
            .Select(m => new { m.Id, m.Price })
            .ToListAsync(cancellationToken);

        if (lines.Count == 0)
            throw new BadRequestException("No payable movies in cart");

        var total = lines.Sum(l => l.Price);

        // Шаг 1: заказ в статусе Pending сохраняется до списания денег
        var order = new OrderEntity
        {
            UserId = request.UserId,
            TotalPrice = total,
            Status = OrderStatus.Pending,
            Items = lines.Select(l => new OrderItemEntity
            {
                MovieId = l.Id,
                Price = l.Price,
                Quantity = 1
            }).ToList(),
            Payment = new PaymentEntity
            {
                Amount = total,
                Status = PaymentStatus.Pending,
                PaymentMethod = PaymentMethod.CreditCard
            }
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        // Шаг 2: оплата
        bool paid;
        try
        {
            paid = total <= 0 || await paymentGateway.PayAsync(request.UserId, total);
        }
        catch
        {
            MarkOrderFailed(order);
            await context.SaveChangesAsync(cancellationToken);
            throw;
        }

        if (!paid)
        {
            MarkOrderFailed(order);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogWarning("Payment failed for user {UserId}, order {OrderId}", request.UserId, order.Id);
            throw new BadRequestException("Payment failed");
        }

        // Шаг 3: всё завершение одним SaveChanges, он атомарен
        order.Status = OrderStatus.Completed;
        order.Payment!.Status = PaymentStatus.Completed;
        order.Payment.ProcessedAt = DateTime.UtcNow;

        foreach (var line in lines)
        {
            context.Purchases.Add(new PurchaseEntity
            {
                UserId = request.UserId,
                MovieId = line.Id,
                PurchasePrice = line.Price,
                PurchasedAt = DateTime.UtcNow
            });
        }

        cart.IsCheckedOut = true;
        cart.CartItems.Clear();

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} completed for user {UserId}, total {Total}",
            order.Id, request.UserId, total);

        return mapper.Map<CartResponse>(cart);
    }

    private static void MarkOrderFailed(OrderEntity order)
    {
        order.Status = OrderStatus.Cancelled;
        if (order.Payment != null)
        {
            order.Payment.Status = PaymentStatus.Failed;
            order.Payment.ProcessedAt = DateTime.UtcNow;
        }
    }
}