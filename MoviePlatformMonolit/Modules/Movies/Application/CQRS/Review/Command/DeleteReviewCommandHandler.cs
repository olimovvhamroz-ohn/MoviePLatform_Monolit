using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Review.Command;

public record DeleteReviewCommand(long ReviewId, long UserId) : IRequest<Unit>;

public class DeleteReviewCommandHandler(ApplicationDbContext context)
    : IRequestHandler<DeleteReviewCommand, Unit>
{
    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.ReviewId <= 0)
            throw new BadRequestException("Review ID must be greater than zero");

        if (request.UserId <= 0)
            throw new BadRequestException("User ID must be greater than zero");

        var review = await context.Reviews
                         .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.ReviewId, cancellationToken)
                     ?? throw new NotFoundException($"Review with ID {request.ReviewId} was not found");

        if (review.UserId != request.UserId)
            throw new UnauthorizedException("You can only delete your own reviews");

        review.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}