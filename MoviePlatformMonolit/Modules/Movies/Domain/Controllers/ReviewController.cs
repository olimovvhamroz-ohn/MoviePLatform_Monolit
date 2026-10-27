using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformMonolit.Modules.Movies.Application.CQRS.Review.Command;
using MoviePlatformMonolit.Modules.Movies.Application.CQRS.Review.Query;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IMediator mediator) : ControllerBase
{
   
    
    [HttpGet("top-movies")]
    public async Task<ActionResult<ApiResponse<List<MovieResponse>>>> GetTopMovies(
        [FromQuery] int page = 1,
        [FromQuery] int count = 10)
    {
        var result = await mediator.Send(new GetTopMoviesQuery(page, count));

        return Ok(ApiResponse<List<MovieResponse>>.SuccessResponse(
            result, $"Retrieved {result.Count} movies"));
    }
    
    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<ApiResponse<List<ReviewResponse>>>> GetMovieReviews(
        int movieId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query =new GetMovieReviewsQuery(movieId, pageNumber, pageSize);
        var result = await mediator.Send(query);
        
        return Ok(ApiResponse<List<ReviewResponse>>.SuccessResponse(result, "Reviews retrieved"));
    }

  
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> AddReview(
        [FromBody] ReviewRequest request)
    {
        if (!long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var result = await mediator.Send(new AddReviewCommand(userId, request));

        return CreatedAtAction(nameof(GetMovieReviews), new { movieId = request.MovieId },
            ApiResponse<ReviewResponse>.SuccessResponse(result, "Review added successfully", 201));
    }

    [Authorize]
    [HttpPut("{reviewId}")]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> UpdateReview(
        long reviewId,
        [FromBody] ReviewRequest request)
    {
        if (!long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var result = await mediator.Send(new UpdateReviewCommand(reviewId, userId, request));

        return Ok(ApiResponse<ReviewResponse>.SuccessResponse(result, "Review updated successfully"));
    }

    [Authorize]
    [HttpDelete("{reviewId}")]
    public async Task<ActionResult<ApiResponse>> DeleteReview(int reviewId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier) ?.Value;
        if(!long.TryParse(userId,out var id)) return Unauthorized();
        var command=new DeleteReviewCommand(reviewId,id);
        await mediator.Send(command);

        return Ok(ApiResponse.SuccessResponse("Review deleted successfully"));
    }
}