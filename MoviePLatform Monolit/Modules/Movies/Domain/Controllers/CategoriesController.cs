using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Movie.CQRS.Categories.Command;
using MoviePLatform_Monolit.Movie.CQRS.Categories.Query;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

 
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryResponse>>>> GetAllCategories()
    {
        
            var query = new GetAllCategories();
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<List<CategoryResponse>>.SuccessResponse(
                result, $"Retrieved {result.Count} categories"));
    }

  
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategoryById(int id)
    {
      
            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(result, "Category retrieved"));
       
    }

   
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> CreateCategory(
        [FromBody] CategoryRequest request)
    {
       
            var command = new CreateCategoryCommand(request) ;
            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id },
                ApiResponse<CategoryResponse>.SuccessResponse(result, "Category created", 201));
      
    }

  
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteCategory(int id)
    {
        
            var command = new DeleteCategoriy(id) ;
            await _mediator.Send(command);
            
            return Ok(ApiResponse.SuccessResponse("Category deleted successfully"));
       
    }
}