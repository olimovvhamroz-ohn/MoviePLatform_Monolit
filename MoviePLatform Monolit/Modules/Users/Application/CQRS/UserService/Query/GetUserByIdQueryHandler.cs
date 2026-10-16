using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.UserService.Query;

public record GetUserByIdQuery(int Id) : IRequest<UserResponse>;

public class GetUserByIdQueryHandler(
    IMapper mapper,
    ILogger<GetUserByIdQueryHandler> logger,
    IUserRepository repository) : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user by id: {Id}", request.Id);

        var user = await repository.GetByIdAsync(request.Id, cancellationToken);

        logger.LogInformation("User found: {Email}", user.Email);

        return mapper.Map<UserResponse>(user);
    }
}