using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.UserService.command;

public record UpdateFavoriteGenresCommand(long UserId, UpdateFavoriteGenresRequest Dto) : IRequest<UserResponse>;

public class UpdateFavoriteGenresCommandHandler(
    IUserRepository repository,
    IMapper mapper,
    ILogger<UpdateFavoriteGenresCommandHandler> logger) : IRequestHandler<UpdateFavoriteGenresCommand, UserResponse>
{
    public async Task<UserResponse> Handle(UpdateFavoriteGenresCommand request, CancellationToken cancellationToken)
    {
        await repository.GetByIdAsync(request.UserId, cancellationToken);

        await repository.SetFavoriteCategoriesAsync(request.UserId, request.Dto.CategoryIds);

        logger.LogInformation("User {UserId} updated favorite genres: {Ids}", request.UserId,
            string.Join(",", request.Dto.CategoryIds));

        var updated = await repository.GetByIdAsync(request.UserId, cancellationToken);
        return mapper.Map<UserResponse>(updated);
    }
}