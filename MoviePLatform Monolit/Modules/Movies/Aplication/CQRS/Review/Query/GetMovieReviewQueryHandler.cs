using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;
using UserService.Domain.Extensions;

namespace MoviePLatform_Monolit.Modules.Movies.Aplication.CQRS.Review.Query;

public  record GetMovieReviewsQuery(long MovieId, int Page, int PageSize)
    : IRequest<List<ReviewResponse>>;
public class GetMovieReviewQueryHandler:IRequestHandler<GetMovieReviewsQuery ,List<ReviewResponse>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetMovieReviewQueryHandler(IReviewRepository repository, IMapper mapper)
    {
        _reviewRepository = repository;
        _mapper = mapper;
    }

    public async Task<List<ReviewResponse>> Handle(GetMovieReviewsQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException(
                "Page and pageSize must be greater than 0");
        }
        var requestSend =await _reviewRepository.GetMovieReviews(request.MovieId, request.Page, request.PageSize);
      return  _mapper.Map<List<ReviewResponse>>(requestSend);
        
    }
    
        
}