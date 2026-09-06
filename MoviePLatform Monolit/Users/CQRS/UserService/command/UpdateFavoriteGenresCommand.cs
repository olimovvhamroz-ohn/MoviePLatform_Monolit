

using MediatR;
using MoviePLatform_Monolit.Users.DTO.REQUEST;

public record UpdateFavoriteGenresCommand(long UserId, UpdateFavoriteGenresRequest Dto) : IRequest<Unit>;

public class UpdateFavoriteGenresCommandHandler : IRequestHandler<UpdateFavoriteGenresCommand, Unit>
{
    private readonly IUserRepository _repository;
    private readonly AppDbcontext _context;
    private readonly ILogger<UpdateFavoriteGenresCommandHandler> _logger;

    public UpdateFavoriteGenresCommandHandler(IUserRepository repository, ILogger<UpdateFavoriteGenresCommandHandler> logger, AppDbcontext context)
    {
        _repository = repository;
        _logger = logger;
        _context = context;
    }

    public async Task<Unit> Handle(UpdateFavoriteGenresCommand request, CancellationToken cancellationToken)
    {
        await _repository.GetById(request.UserId); // кинет NotFoundException агар набошад
        var user= await _context.
        await _repository.SetFavoriteCategoriesAsync(request.UserId, request.Dto.CategoryIds);

        _logger.LogInformation("User {UserId} updated favorite genres: {Ids}", request.UserId, string.Join(",", request.Dto.CategoryIds));

        return Unit.Value;
    }
}