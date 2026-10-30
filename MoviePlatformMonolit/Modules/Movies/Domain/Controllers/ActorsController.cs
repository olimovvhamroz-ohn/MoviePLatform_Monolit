using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformMonolit.Modules.Movies.Application.CQRS.Actor.Command;
using MoviePlatformMonolit.Modules.Movies.Application.CQRS.Actor.Query;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActorsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> GetActorById(long id)
    {
        var query = new GetByIdActorQuery(id);
        var result = await mediator.Send(query);

        if (result == null)
            return NotFound(ApiResponse<ActorResponse>.NotFoundResponse("Actor not found"));

        return Ok(ApiResponse<ActorResponse>.SuccessResponse(result, "Actor retrieved"));
    }
    
    [HttpGet("search/{name}")]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> GetActorByName(string name)
    {
        var query = new GetActorByNameQuery(name);
        var result = await mediator.Send(query);
        return Ok(ApiResponse<ActorResponse>.SuccessResponse(result, "Actor found"));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> CreateActor(
        [FromBody] ActorRequest request)
    {
        var command = new CreateCommand(request);
        
        var result = await mediator.Send(command);
        
        return CreatedAtAction(nameof(GetActorById), new { id = result.Id },
            ApiResponse<ActorResponse>.SuccessResponse(result, "Actor created", 201));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteActor(int id)
    {
        var command = new DeleteCommand(id);
        await mediator.Send(command);

        return Ok(ApiResponse.SuccessResponse("Actor deleted successfully"));
    }
}