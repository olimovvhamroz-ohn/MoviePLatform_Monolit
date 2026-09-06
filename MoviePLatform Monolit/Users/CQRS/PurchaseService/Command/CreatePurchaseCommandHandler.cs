

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Movie.Repositories;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record CreatePurchaseCommand(PurchaseRequest PurchaseData) : IRequest<PurchaseResponse>;

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, PurchaseResponse>
{
    private readonly IPurchaseRepository _repository;
    private readonly IMovieRepository _movie; // ✅ вместо AppDbContext
    private readonly IMapper _mapper;

    public CreatePurchaseCommandHandler(
        IPurchaseRepository repository,
        IMovieRepository movieReadModel,
        IMapper mapper)
    {
        _repository = repository;
        _movie = movieReadModel;
        _mapper = mapper;
    }

    public async Task<PurchaseResponse> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var dto = request.PurchaseData;

        if (dto.UserId <= 0 || dto.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var movie = await _movie.GetByIdAsync(dto.MovieId, cancellationToken);
        if (movie == null)
            throw new NotFoundException($"Movie {dto.MovieId} not found");

        var alreadyBought = await _repository.HasUserPurchasedMovie(dto.UserId, dto.MovieId);
        if (alreadyBought)
            throw new BadRequestException("Already purchased");

        var model = _mapper.Map<PurchaseEntity>(dto);
        model.MovieId = movie.Id;
        model.PurchasedAt = DateTime.UtcNow;

        var created = await _repository.CreatePurchase(model);

        return _mapper.Map<PurchaseResponse>(created);
    }
}