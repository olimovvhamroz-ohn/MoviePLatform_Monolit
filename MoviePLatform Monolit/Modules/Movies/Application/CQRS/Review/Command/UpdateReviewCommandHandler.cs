using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Command;

public record UpdateReviewCommand(long ReviewId, long UserId, ReviewRequest Request) : IRequest<ReviewResponse>;

public class UpdateReviewCommandHandler(
    ApplicationDbContext context,
    IMapper mapper) : IRequestHandler<UpdateReviewCommand, ReviewResponse>
{
    public async Task<ReviewResponse> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.ReviewId <= 0)
            throw new BadRequestException("Review ID must be greater than zero");

        if (request.UserId <= 0)
            throw new BadRequestException("User ID must be greater than zero");

        if (request.Request == null)
            throw new BadRequestException("Review data can not be null");

        var review = await context.Reviews
                         .FirstOrDefaultAsync(x => x.Id == request.ReviewId, cancellationToken)
                     ?? throw new NotFoundException($"Review with ID {request.ReviewId} was not found");

        if (review.UserId != request.UserId)
            throw new UnauthorizedException("You can only edit your own reviews");

        review.Comment = request.Request.Comment;
        review.Rating = request.Request.Rating;

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ReviewResponse>(review);
    }
}