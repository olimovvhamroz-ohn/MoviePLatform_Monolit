using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.UserService.Query;

public record GetAllUsersQuery : IRequest<List<UserResponse>>;

public class GetAllUsersQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<GetAllUsersQueryHandler> logger) : IRequestHandler<GetAllUsersQuery, List<UserResponse>>
{
    public async Task<List<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all users");

        var users = await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        logger.LogInformation("Total users: {Count}", users.Count);

        return mapper.Map<List<UserResponse>>(users);
    }
}