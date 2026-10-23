using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.Request;
using MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.Response;
using MoviePLatform_Monolit.Modules.Users.Application.Policies;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.PurchaseService.Command;

public record CreatePurchaseCommand(PurchaseRequest PurchaseData) : IRequest<PurchaseResponse>;

public class CreatePurchaseCommandHandler(
    ApplicationDbContext context,
    IMapper mapper) : IRequestHandler<CreatePurchaseCommand, PurchaseResponse>
{
    public async Task<PurchaseResponse> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var dto = request.PurchaseData;

        if (dto.UserId <= 0 || dto.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var userExists = await context.Users
            .AnyAsync(x => x.Id == dto.UserId, cancellationToken);
        if (!userExists)
            throw new NotFoundException($"User {dto.UserId} not found");

        var movieExists = await context.Movies
            .AnyAsync(x => x.Id == dto.MovieId, cancellationToken);
        if (!movieExists)
            throw new NotFoundException($"Movie {dto.MovieId} not found");

        var alreadyBought = await context.Purchases
            .AnyAsync(x => x.UserId == dto.UserId && x.MovieId == dto.MovieId, cancellationToken);
        if (alreadyBought)
            throw new BadRequestException("Already purchased");

        var entity = new PurchaseEntity
        {
            UserId = dto.UserId,
            MovieId = dto.MovieId,
            PurchasedAt = DateTime.UtcNow,
            PurchasePrice = 0
        };

        context.Purchases.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<PurchaseResponse>(entity);
        response.RentalStartDeadline = RentalPolicy.GetStartDeadline(entity.PurchasedAt);
        return response;
    }
}