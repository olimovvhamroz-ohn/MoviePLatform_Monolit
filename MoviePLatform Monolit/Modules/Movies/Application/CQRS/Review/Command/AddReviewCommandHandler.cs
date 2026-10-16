using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Command;

public record AddReviewCommand(long UserId, long MovieId, ReviewResponse Dto) : IRequest<ReviewResponse>;

public class AddReviewCommandHandler(
    IMapper mapper,
    IReviewRepository reviewRepository,
    IMovieRepository movieRepository,
    IUserRepository userRepository) : IRequestHandler<AddReviewCommand, ReviewResponse>
{
    public async Task<ReviewResponse> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        await movieRepository.GetByIdAsync(request.MovieId, cancellationToken);
        await userRepository.GetByIdAsync(request.UserId, cancellationToken);


        var entity = mapper.Map<ReviewEntity>(request.Dto);
        entity.MovieId = request.MovieId;
        entity.UserId = request.UserId;


        var created = await reviewRepository.CreateAsync(entity, cancellationToken);


        return mapper.Map<ReviewResponse>(created);
    }
}