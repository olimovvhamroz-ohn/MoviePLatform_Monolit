using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MoviePLatform_Monolit.Modules.Engagement.Domain.Controllers;

[ApiController]
[Route("api/trending")]
public class TrendingController : ControllerBase
{
    private readonly IMediator _mediator;

    public TrendingController(IMediator mediator)
    {
        _mediator = mediator;
    }

  
  
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TrendingMovieResponse>>>> GetTrendingMovies(
        [FromQuery] int take = 10,
        [FromQuery] int days = 7)
    {
            var query = new GetTrendingQuery(days, take);
        

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<TrendingMovieResponse>>.SuccessResponse(
                result, $"Retrieved {result.Count} trending movies"));
       
    }

   
    [HttpGet("weekly")]
    public async Task<ActionResult<ApiResponse<List<TrendingMovieResponse>>>> GetWeeklyTrending(
        [FromQuery] int count = 10)
    {
        
            var query = new GetTrendingQuery(7, count);

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<TrendingMovieResponse>>.SuccessResponse(
                result, "Weekly trending movies retrieved"));
        
       
        
    }

    /// <summary>
    /// Получить популярные фильмы за месяц
    /// </summary>
    [HttpGet("monthly")]
    public async Task<ActionResult<ApiResponse<List<TrendingMovieResponse>>>> GetMonthlyTrending(
        [FromQuery] int count = 10)
    {
            var query = new GetTrendingQuery(30, count);

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<TrendingMovieResponse>>.SuccessResponse(
                result, "Monthly trending movies retrieved"));
       
    }

    
  
  
    [HttpGet("all-time")]
    public async Task<ActionResult<ApiResponse<List<TrendingMovieResponse>>>> GetAllTimeTrending(
        [FromQuery] int count = 10)
    {
       
            var query = new GetTrendingQuery(9999, count);
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<TrendingMovieResponse>>.SuccessResponse(
                result, "All-time trending movies retrieved"));
        
      
    }

 
   
  
    [HttpGet("stats/{movieId}")]
    public ActionResult<ApiResponse<object>> GetMovieStats(int movieId)
    {
        
            // Implement movie stats logic
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