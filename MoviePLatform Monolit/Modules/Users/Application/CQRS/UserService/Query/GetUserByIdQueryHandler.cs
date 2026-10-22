using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.UserService.Query;

public record GetUserByIdQuery(long Id) : IRequest<UserResponse>;

public class GetUserByIdQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<GetUserByIdQueryHandler> logger) : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        logger.LogInformation("Getting user by id: {Id}", request.Id);

        var user = await context.Users
                       .AsNoTracking()
                       .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                   ?? throw new NotFoundException($"User {request.Id} not found");

        return mapper.Map<UserResponse>(user);
    }
}