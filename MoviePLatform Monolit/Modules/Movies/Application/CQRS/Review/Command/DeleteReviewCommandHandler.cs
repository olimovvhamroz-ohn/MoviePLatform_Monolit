using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Command;

public record DeleteReviewCommand(long ReviewId, long UserId) : IRequest<Unit>;

public class DeleteReviewCommandHandler(ApplicationDbContext context) : IRequestHandler<DeleteReviewCommand, Unit>
{
    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.ReviewId <= 0) throw new BadRequestException("ID must be greater than zero");

        if (request.UserId <= 0)
            throw new BadRequestException("User ID must be greater than zero");

        var res = await context.Reviews.FirstOrDefaultAsync(x => x.Id == request.ReviewId, cancellationToken)
                  ?? throw new NotFoundException($"Id {request.ReviewId} not found");
        
        if (res.UserId != request.UserId)
        {
            throw new UnauthorizedException(" иҷозат надорed");
        }

        context.Reviews.Remove(res);
        await context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}