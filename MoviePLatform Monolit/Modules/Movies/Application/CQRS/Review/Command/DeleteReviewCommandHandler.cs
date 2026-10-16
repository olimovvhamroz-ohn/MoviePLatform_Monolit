using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Command;

public record DeleteReviewCommand(long ReviewId, long UserId) : IRequest<Unit>;

public class DeleteReviewCommandHandler(IReviewRepository review) : IRequestHandler<DeleteReviewCommand, Unit>
{
    
    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var result = await review.GetByIdAsync(
            request.ReviewId,
            cancellationToken);
        
        if (result.UserId != request.UserId)
        {
            throw new UnauthorizedException(" иҷозат надорed");
        }

        await review.DeleteAsync(request.ReviewId, cancellationToken);
        return Unit.Value;
    }
}