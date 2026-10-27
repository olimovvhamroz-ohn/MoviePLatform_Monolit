using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Review.Command;

public record AddReviewCommand(long UserId, ReviewRequest Request) : IRequest<ReviewResponse>;

public class AddReviewCommandHandler(
    ApplicationDbContext context,
    IMapper mapper) : IRequestHandler<AddReviewCommand, ReviewResponse>
{
    public async Task<ReviewResponse> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
            throw new BadRequestException("User ID must be greater than zero");

        if (request.Request is null)
            throw new BadRequestException("Review can not be null");

        if (request.Request.MovieId <= 0)
            throw new BadRequestException("Movie ID must be greater than zero");

        var movieExists = await context.Movies
            .AnyAsync(x => !x.IsDeleted && x.Id == request.Request.MovieId, cancellationToken);

        if (!movieExists)
            throw new NotFoundException($"Movie with ID {request.Request.MovieId} was not found");

        var userExists = await context.Users
            .AnyAsync(x => !x.IsDeleted && x.Id == request.UserId, cancellationToken);

        if (!userExists)
            throw new NotFoundException($"User with ID {request.UserId} was not found");

        var entity = mapper.Map<ReviewEntity>(request.Request);
        entity.UserId = request.UserId;

        context.Reviews.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ReviewResponse>(entity);
    }
}