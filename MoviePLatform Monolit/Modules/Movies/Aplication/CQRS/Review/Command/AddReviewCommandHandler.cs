
using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Reviews.Command;

public record AddReviewCommand( long UserId,long MovieId,ReviewResponse Dto) : IRequest<ReviewResponse>;
public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, ReviewResponse>
{
    private readonly IMapper _mapper;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IUserRepository _userRepository;

    public AddReviewCommandHandler(
        IMapper mapper, 
        IReviewRepository reviewRepository, 
        IMovieRepository movieRepository,
        IUserRepository userRepository)
    {
        _mapper = mapper;
        _reviewRepository = reviewRepository;
        _movieRepository = movieRepository;
        _userRepository = userRepository;
    }

    public async Task<ReviewResponse> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
       
        await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken);
        await _userRepository.GetById(request.UserId);

        
        var entity = _mapper.Map<ReviewEntity>(request.Dto);
        entity.MovieId = request.MovieId;
        entity.UserId = request.UserId;

       
        var created = await _reviewRepository.CreateAsync(entity, cancellationToken);

   
        return _mapper.Map<ReviewResponse>(created);
    }
}