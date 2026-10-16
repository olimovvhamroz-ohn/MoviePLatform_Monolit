using System.Security.Authentication;
using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Command;

public record UpdateReviewCommand(long ReviewId, long UserId, ReviewRequest Request) : IRequest<ReviewResponse>;

public class UpdateReviewCommandHandler(IReviewRepository reviewRepository,IMapper mapper) : IRequestHandler<UpdateReviewCommand, ReviewResponse>
{

    public async Task<ReviewResponse> Handle(UpdateReviewCommand reviewCommand, CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetByIdAsync(reviewCommand.ReviewId,cancellationToken);
        if (review.UserId != reviewCommand.UserId)
        {
            throw new AuthenticationException("error");
        }

        review.Comment = reviewCommand.Request.Comment;
        review.Rating= reviewCommand.Request.Rating;
        var updateeview = await reviewRepository.UpdateAsync(review, cancellationToken);
        return mapper.Map<ReviewResponse>(updateeview);    }
}
