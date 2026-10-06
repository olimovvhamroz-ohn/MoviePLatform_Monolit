using MediatR;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;
using UserService.Domain.Extensions;

namespace MoviePLatform_Monolit.Modules.Movies.Aplication.CQRS.Review.Command;

public record DeleteReviewCommand(long ReviewId, long UserId):IRequest<Unit>;
public class DeleteReviewCommandHandler:IRequestHandler<DeleteReviewCommand,Unit>
{
    private readonly IReviewRepository _review;

    public DeleteReviewCommandHandler(IReviewRepository review)
    {
        _review = review;
    }

    public async  Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _review.GetByIdAsync(
            request.ReviewId,
            cancellationToken);
        if (review.UserId != request.UserId)
        {
            throw new UnauthorizedException(" иҷозат надорed");
        }

        await _review.DeleteAsync(request.ReviewId, cancellationToken);
        return Unit.Value;
    }
}