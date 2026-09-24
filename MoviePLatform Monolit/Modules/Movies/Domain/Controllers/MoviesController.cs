

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Movie.CQRS.Movies.Command;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoviesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> GetMovieById(int id)
    {
        
            var query = new GetMovieByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(ApiResponse<MovieResponse>.NotFoundResponse("Movie not found"));

            return Ok(ApiResponse<MovieResponse>.SuccessResponse(result, "Movie retrieved successfully"));
       
    }

  
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<MovieResponse>>>> GetMovies(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
       
            var query = new GetMoviesPaginationQuery(pageNumber, pageSize);
               
        
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<MovieResponse>>.SuccessResponse(result, "Movies retrieved successfully"));

       
    }

  
    
   
    [HttpGet("search/{name}")]
    public async Task<ActionResult<ApiResponse<List<MovieResponse>>>> SearchMovies(string name)
    {
       
            var query = new GetMoviesByNameQuery(name);
            var result = await _mediator.Send(query);
        
            return Ok(ApiResponse<List<MovieResponse>>.SuccessResponse(
                result, $"Found {result.Count} movies"));
        
    }

   
    
  
    [HttpPost("filter")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<MovieResponse>>>> FilterMovies(
        [FromBody] MovieFilterRequest filter)
    {
      
        var query = new GetMoviesByFilterQuery(filter.CategoryId, filter.Year, filter.MaxPrice, filter.AgeRating);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<MovieResponse>>.SuccessResponse(result, "Filtered movies retrieved"));
    }

   

 
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> CreateMovie(
        [FromBody] MovieRequest request)
    {
       
            var command = new CreateMovieCommand
                (request);
           

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMovieById), new { id = result.Id }, 
                ApiResponse<MovieResponse>.SuccessResponse(result, "Movie created successfully", 201));
       
    }

  
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> UpdateMovie(
        int id, [FromBody] MovieRequest request)
    {
        
            var command = new UpdateMovieCommand(id, request);
          

            var result = await _mediator.Send(command);
            return Ok(ApiResponse<MovieResponse>.SuccessResponse(result, "Movie updated successfully"));
       
    }

   
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteMovie(int id)
    {
      
        var command = new DeleteMovieCommand (id);
            await _mediator.Send(command);
            
            return Ok(ApiResponse.SuccessResponse("Movie deleted successfully"));
      
    }
}