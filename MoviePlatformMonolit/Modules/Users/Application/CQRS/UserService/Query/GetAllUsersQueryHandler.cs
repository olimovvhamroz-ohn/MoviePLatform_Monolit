using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Users.Application.CQRS.UserService.Query;

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
            .Where(x=>x.IsDeleted==false)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Total users: {Count}", users.Count);

        return mapper.Map<List<UserResponse>>(users);
    }
}