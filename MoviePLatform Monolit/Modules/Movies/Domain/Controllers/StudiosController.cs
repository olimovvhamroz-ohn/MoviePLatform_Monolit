using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Movie.CQRS.Studios.Command;
using MoviePLatform_Monolit.Movie.CQRS.Studios.Query;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;


namespace MoviePLatform_Monolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudiosController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudiosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StudioResponse>>>> GetAllStudios()
    {
        
            var query = new GetAllStudiosQuery();
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<List<StudioResponse>>.SuccessResponse(
                result, $"Retrieved {result.Count} studios"));
      
    }

  
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StudioResponse>>> GetStudioById(int id)
    {
      
            var query = new GetStudioByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(ApiResponse<StudioResponse>.NotFoundResponse("Studio not found"));

            return Ok(ApiResponse<StudioResponse>.SuccessResponse(result, "Studio retrieved"));
       
    }


  
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudioResponse>>> CreateStudio(
        [FromBody] StudioRequest request)
    {
       
            var command = new CreateStudioCommand(request);
            
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetStudioById), new { id = result.Id },
                ApiResponse<StudioResponse>.SuccessResponse(result, "Studio created", 201));
       
    }

   
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteStudio(int id)
    {
        
            var command = new DeleteStudioCommand (id);
            await _mediator.Send(command);
            
            return Ok(ApiResponse.SuccessResponse("Studio deleted successfully"));
       
    }
}