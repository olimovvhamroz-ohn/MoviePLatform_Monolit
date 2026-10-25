using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.UserService.command;
public record UpdateUserCommand(long Id, UpdateProfileRequest Request) : IRequest<UserResponse>;

    public class UpdateUserCommandHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<UpdateUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        if (request.Request == null)
            throw new BadRequestException("Profile data can not be null");

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"User {request.Id} not found");

        if (request.Request.Name is not null)
        {
            var name = request.Request.Name.Trim();
            if (name.Length == 0)
                throw new BadRequestException("Name can not be empty");
            user.Name = name;
        }

        if (request.Request.Phone is not null)
        {
            var phone = request.Request.Phone.Trim();
            if (phone.Length == 0)
                throw new BadRequestException("Phone can not be empty");
            user.Phone = phone;
        }

        if (request.Request.Email is not null)
        {
            var email = request.Request.Email.Trim().ToLowerInvariant();
            if (email.Length == 0)
                throw new BadRequestException("Email can not be empty");

            if (email != user.Email)
            {
                var taken = await context.Users
                    .AnyAsync(x => x.Email == email && x.Id != request.Id, cancellationToken);

                if (taken)
                    throw new BadRequestException("Email is already in use");

                user.Email = email;
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} updated profile", request.Id);

        return mapper.Map<UserResponse>(user);
    }
}


