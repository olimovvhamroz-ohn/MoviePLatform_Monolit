using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoviePLatform_Monolit.Modules.Movies.Aplication.CQRS.Review.Query;
using MoviePLatform_Monolit.Movie.CQRS.Reviews.Command;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("top-movies")]
    public async Task<ActionResult<ApiResponse<List<MovieResponse>>>> GetTopMovies(
        [FromQuery] int count = 10)
    {
        var query = new GetTopMoviesQuery(1, count);
        var result = await _mediator.Send(query);

        return Ok(ApiResponse<List<MovieResponse>>.SuccessResponse(
            result, $"Retrieved top {count} movies"));
    }

    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<ApiResponse<List<ReviewResponse>>>> GetMovieReviews(
        int movieId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query =new GetMovieReviewsQuery(movieId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        
        return Ok(ApiResponse<List<ReviewResponse>>.SuccessResponse(result, "Reviews retrieved"));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> AddReview(
        [FromBody] ReviewRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var command = new AddReviewCommand(id, request.MovieId, new ReviewResponse
        {
            UserId = id,
            Rating = request.Rating,
            Comment = request.Comment
        });

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMovieReviews), new { movieId = request.MovieId },
            ApiResponse<ReviewResponse>.SuccessResponse(result, "Review added successfully", 201));
    }

    [Authorize]
    [HttpPut("{reviewId}")]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> UpdateReview(
        int reviewId,
        [FromBody] ReviewRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        // TODO: нужна отдельная UpdateReviewCommand с проверкой автора/reviewId
        var command = new AddReviewCommand(id, request.MovieId, new ReviewResponse
        {
            UserId = id,
            Rating = request.Rating,
            Comment = request.Comment
        });

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<ReviewResponse>.SuccessResponse(result, "Review updated successfully"));
    }

    [Authorize]
    [HttpDelete("{reviewId}")]
    public async Task<ActionResult<ApiResponse>> DeleteReview(int reviewId)
    {
        // TODO: реализовать DeleteReviewCommand
        return Ok(ApiResponse.SuccessResponse("Review deleted successfully"));
    }
}