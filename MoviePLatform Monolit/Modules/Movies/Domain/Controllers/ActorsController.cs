using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Movie.CQRS.Actor.Command;
using MoviePLatform_Monolit.Movie.CQRS.Actor.Query;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;


namespace MoviePLatform_Monolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ActorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

  
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> GetActorById(int id)
    {
        
            var query = new GetByIdActorQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(ApiResponse<ActorResponse>.NotFoundResponse("Actor not found"));

            return Ok(ApiResponse<ActorResponse>.SuccessResponse(result, "Actor retrieved"));
      
    }

   
    [HttpGet("search/{name}")]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> GetActorByName(string name)
    {
       
            var query = new GetActorByNameQuery(name);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(ApiResponse<ActorResponse>.NotFoundResponse("Actor not found"));

            return Ok(ApiResponse<ActorResponse>.SuccessResponse(result, "Actor found"));
      
    }

   
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> CreateActor(
        [FromBody] ActorRequest request)
    {
        
            var command = new CreateCommand(request);
            
            
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetActorById), new { id = result.Id },
                ApiResponse<ActorResponse>.SuccessResponse(result, "Actor created", 201));
        
    
    }

    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteActor(int id)
    {
       
            var command = new DeleteCommand(id);
            await _mediator.Send(command);
            
            return Ok(ApiResponse.SuccessResponse("Actor deleted successfully"));
       
    }
}