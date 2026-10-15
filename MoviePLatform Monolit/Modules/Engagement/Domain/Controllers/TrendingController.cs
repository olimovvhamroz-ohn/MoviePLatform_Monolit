using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Query;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Domain.Controllers;

[ApiController]
[Route("api/trending")]
public class TrendingController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TrendingMovieResponse>>>> GetTrendingMovies(
        [FromQuery] int take = 10,
        [FromQuery] int days = 7)
    {
        var query = new GetTrendingQuery(days, take);

        var result = await mediator.Send(query);

        return Ok(ApiResponse<List<TrendingMovieResponse>>.SuccessResponse(
            result, $"Retrieved {result.Count} trending movies"));
    }

    [HttpGet("weekly")]
    public async Task<ActionResult<ApiResponse<List<TrendingMovieResponse>>>> GetWeeklyTrending(
        [FromQuery] int count = 10)
    {
        var query = new GetTrendingQuery(7, count);

        var result = await mediator.Send(query);
        return Ok(ApiResponse<List<TrendingMovieResponse>>.SuccessResponse(
            result, "Weekly trending movies retrieved"));
    }

   
    [HttpGet("monthly")]
    public async Task<ActionResult<ApiResponse<List<TrendingMovieResponse>>>> GetMonthlyTrending(
        [FromQuery] int count = 10)
    {
        var query = new GetTrendingQuery(30, count);

        var result = await mediator.Send(query);

        return Ok(ApiResponse<List<TrendingMovieResponse>>.SuccessResponse(
            result, "Monthly trending movies retrieved"));
    }


    [HttpGet("all-time")]
    public async Task<ActionResult<ApiResponse<List<TrendingMovieResponse>>>> GetAllTimeTrending(
        [FromQuery] int count = 10)
    {
        var query = new GetTrendingQuery(9999, count);
        
        var result = await mediator.Send(query);
        
        return Ok(ApiResponse<List<TrendingMovieResponse>>.SuccessResponse(
            result, "All-time trending movies retrieved"));
    }
    
    [HttpGet("stats/{movieId}")]
    public ActionResult<ApiResponse<object>> GetMovieStats(int movieId)
    {
        var stats = new
        {
            MovieId = movieId,
            TotalViews = 0,
            TrendingScore = 0.0,
            Rank = 0,
            LastUpdated = DateTime.UtcNow
        };

        return Ok(ApiResponse<object>.SuccessResponse(stats, "Movie stats retrieved"));
    }
}