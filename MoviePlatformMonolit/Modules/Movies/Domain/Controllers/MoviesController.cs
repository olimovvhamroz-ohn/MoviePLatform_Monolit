using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Command;
using MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Query;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<MovieResponse>>>> GetMovies(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetMoviesPaginationQuery(pageNumber, pageSize));

        return Ok(ApiResponse<PaginatedResponse<MovieResponse>>.SuccessResponse(
            result, "Movies retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> GetMovieById(long id)
    {
        var result = await mediator.Send(new GetMovieByIdQuery(id));

        return Ok(ApiResponse<MovieResponse>.SuccessResponse(result, "Movie retrieved successfully"));
    }

    [HttpGet("search/{name}")]
    public async Task<ActionResult<ApiResponse<List<MovieResponse>>>> SearchMovies(string name)
    {
        var result = await mediator.Send(new GetMoviesByNameQuery(name));

        return Ok(ApiResponse<List<MovieResponse>>.SuccessResponse(
            result, $"Found {result.Count} movies"));
    }

    [HttpPost("filter")]
    public async Task<ActionResult<ApiResponse<List<MovieResponse>>>> FilterMovies(
        [FromBody] MovieFilterRequest filter)
    {
        var query = new GetMoviesByFilterQuery(
            filter.CategoryId, filter.Year, filter.MaxPrice, filter.AgeRating);

        var result = await mediator.Send(query);

        return Ok(ApiResponse<List<MovieResponse>>.SuccessResponse(
            result, "Filtered movies retrieved"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> CreateMovie(
        [FromBody] MovieRequest request)
    {
        var result = await mediator.Send(new CreateMovieCommand(request));

        return CreatedAtAction(nameof(GetMovieById), new { id = result.Id },
            ApiResponse<MovieResponse>.SuccessResponse(result, "Movie created successfully", 201));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> UpdateMovie(
        long id, [FromBody] MovieRequest request)
    {
        var result = await mediator.Send(new UpdateMovieCommand(id, request));

        return Ok(ApiResponse<MovieResponse>.SuccessResponse(result, "Movie updated successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteMovie(long id)
    {
        await mediator.Send(new DeleteMovieCommand(id));

        return Ok(ApiResponse.SuccessResponse("Movie deleted successfully"));
    }
}