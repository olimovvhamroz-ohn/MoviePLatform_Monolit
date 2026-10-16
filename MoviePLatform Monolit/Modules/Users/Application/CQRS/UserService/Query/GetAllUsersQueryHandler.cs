using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.UserService.Query;

public record GetAllUsersQuery : IRequest<List<UserResponse>>;

public class GetAllUsersQueryHandler(
    IMapper mapper,
    ILogger<GetAllUsersQueryHandler> logger,
    IUserRepository repository) : IRequestHandler<GetAllUsersQuery, List<UserResponse>>
{
    public async Task<List<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all users");

        var users = await repository.GetAllAsync();

        logger.LogInformation("Total users: {Count}", users.Count);

        return mapper.Map<List<UserResponse>>(users);
    }
}