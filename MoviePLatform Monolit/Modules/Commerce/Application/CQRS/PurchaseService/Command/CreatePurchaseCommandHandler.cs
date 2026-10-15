using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.PurchaseService.Command;

public record CreatePurchaseCommand(PurchaseRequest PurchaseData) : IRequest<PurchaseResponse>;

public class CreatePurchaseCommandHandler(
    IPurchaseRepository repository,
    IMovieRepository movieReadModel,
    IMapper mapper) : IRequestHandler<CreatePurchaseCommand, PurchaseResponse>
{
  

    public async Task<PurchaseResponse> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var dto = request.PurchaseData;

        if (dto.UserId <= 0 || dto.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var movie = await movieReadModel.GetByIdAsync(dto.MovieId, cancellationToken);
        if (movie == null)
            throw new NotFoundException($"Movie {dto.MovieId} not found");

        var alreadyBought = await repository.HasUserPurchasedMovie(dto.UserId, dto.MovieId);
        if (alreadyBought)
            throw new BadRequestException("Already purchased");

        var model = mapper.Map<PurchaseEntity>(dto);
        model.MovieId = movie.Id;
        model.PurchasedAt = DateTime.UtcNow;
        model.PurchasePrice = 0;

        var created = await repository.CreatePurchase(model);

        return mapper.Map<PurchaseResponse>(created);
    }
}