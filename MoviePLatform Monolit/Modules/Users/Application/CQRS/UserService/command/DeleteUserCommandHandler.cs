using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.UserService.command;

public record DeleteUserCommand(long Id) : IRequest<Unit>;

public class DeleteUserCommandHandler(ApplicationDbContext context) : IRequestHandler<DeleteUserCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        var user = await context.Users.Where(x=>x.IsDeleted==false)
                       .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                   ?? throw new NotFoundException($"User {request.Id} not found");
        user.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}