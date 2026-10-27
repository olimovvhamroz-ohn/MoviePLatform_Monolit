using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformMonolit.Modules.Movies.Application.CQRS.Categories.Command;
using MoviePlatformMonolit.Modules.Movies.Application.CQRS.Categories.Query;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryResponse>>>> GetAllCategories()
    {
        
            var query = new GetAllCategories();
            var result = await mediator.Send(query);
            
            return Ok(ApiResponse<List<CategoryResponse>>.SuccessResponse(
                result, $"Retrieved {result.Count} categories"));
    }

  
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategoryById(int id)
    {
      
            var query = new GetCategoryByIdQuery(id);
            var result = await mediator.Send(query);
            
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(result, "Category retrieved"));
       
    }

   
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> CreateCategory(
        [FromBody] CategoryRequest request)
    {
       
            var command = new CreateCategoryCommand(request) ;
            var result = await mediator.Send(command);
            
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id },
                ApiResponse<CategoryResponse>.SuccessResponse(result, "Category created", 201));
      
    }

  
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteCategory(int id)
    {
        
            var command = new DeleteCategoryCommand(id) ;
            await mediator.Send(command);
            
            return Ok(ApiResponse.SuccessResponse("Category deleted successfully"));
       
    }
}