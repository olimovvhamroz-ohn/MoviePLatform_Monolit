using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Command;

public record AddReviewCommand(long UserId, long MovieId, ReviewResponse Review) : IRequest<ReviewResponse>;

public class AddReviewCommandHandler(
    IMapper mapper,
    ApplicationDbContext context) : IRequestHandler<AddReviewCommand, ReviewResponse>
{
    public async Task<ReviewResponse> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.Review == null)
            throw new BadRequestException("Review can not be null");

        if (request.MovieId <= 0)
            throw new BadRequestException("Movie ID must be greater than zero");

        if (request.UserId <= 0)
            throw new BadRequestException("User ID must be greater than zero");

        var movieExists = await context.Movies
            .AnyAsync(x => x.Id == request.MovieId, cancellationToken);

        if (!movieExists)
            throw new NotFoundException($"Movie with ID {request.MovieId} was not found");

        var userExists = await context.Users
            .AnyAsync(x => x.Id == request.UserId, cancellationToken);

        if (!userExists)
            throw new NotFoundException($"User with ID {request.UserId} was not found");

        var entity = mapper.Map<ReviewEntity>(request.Review);
        entity.MovieId = request.MovieId;
        entity.UserId = request.UserId;

        await context.Reviews.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ReviewResponse>(entity);
    }
}