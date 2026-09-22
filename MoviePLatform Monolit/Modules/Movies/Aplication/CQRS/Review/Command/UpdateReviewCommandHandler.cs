using System.Security.Authentication;
using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Aplication.CQRS.Review.Command;

public record UpdateReviewCommand(long ReviewId, long UserId, ReviewRequest Request) : IRequest<ReviewResponse>;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
   

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository,IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper=mapper;
        
    }

    public async Task<ReviewResponse> Handle(UpdateReviewCommand reviewCommand, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewCommand.ReviewId,cancellationToken);
        if (review.UserId != reviewCommand.UserId)
        {
            throw new AuthenticationException("error");
        }

        review.Comment = reviewCommand.Request.Comment;
        review.Rating= reviewCommand.Request.Rating;
        var updateeview = await _reviewRepository.UpdateAsync(review, cancellationToken);
        return _mapper.Map<ReviewResponse>(updateeview);    }
}
