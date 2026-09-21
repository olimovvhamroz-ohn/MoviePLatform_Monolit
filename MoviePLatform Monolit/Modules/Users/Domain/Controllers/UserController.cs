using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Domain.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetAllUsers()
    {
        var query = new GetAllUsersQuery();
        var result = await _mediator.Send(query);

        return Ok(ApiResponse<List<UserResponse>>.SuccessResponse(
            result, $"Retrieved {result.Count} users"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserById(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != id.ToString() && !User.IsInRole("Admin"))
            return Forbid();

        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(ApiResponse<UserResponse>.NotFoundResponse("User not found"));

        return Ok(ApiResponse<UserResponse>.SuccessResponse(result, "User retrieved"));
    }

    [HttpGet("me/profile")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized();

        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(ApiResponse<UserResponse>.NotFoundResponse("User not found"));

        return Ok(ApiResponse<UserResponse>.SuccessResponse(result, "Current user retrieved"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser(
        int id, [FromBody] UserRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != id.ToString() && !User.IsInRole("Admin"))
            return Forbid();

        // TODO: реализовать UpdateUserCommand
        return Ok(ApiResponse<UserResponse>.SuccessResponse(
            new UserResponse(), "User updated successfully"));
    }

    [HttpPut("{id}/favorite-genres")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateFavoriteGenres(
        int id, [FromBody] UpdateFavoriteGenresRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != id.ToString())
            return Forbid();

        var command = new UpdateFavoriteGenresCommand(id, request);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(result, "Favorite genres updated"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteUser(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != id.ToString() && !User.IsInRole("Admin"))
            return Forbid();

        // TODO: реализовать DeleteUserCommand
        return Ok(ApiResponse.SuccessResponse("User account deleted successfully"));
    }
}