using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Users.Application.CQRS.UserService.command;
using MoviePlatformMonolit.Modules.Users.Application.CQRS.UserService.Query;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Users.Domain.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IMediator mediator) : ControllerBase
{
    private bool TryGetCurrentUserId(out long id) =>
        long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out id) && id > 0;

    private bool IsSelfOrAdmin(long id) =>
        (TryGetCurrentUserId(out var currentId) && currentId == id) || User.IsInRole("Admin");

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetAllUsers(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllUsersQuery(), ct);
        return Ok(ApiResponse<List<UserResponse>>.SuccessResponse(result, $"Retrieved {result.Count} users"));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserById(long id, CancellationToken ct)
    {
        if (!IsSelfOrAdmin(id))
            return Forbid();

        var result = await mediator.Send(new GetUserByIdQuery(id), ct);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(result, "User retrieved"));
    }

    [HttpGet("me/profile")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetCurrentUser(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var id))
            return Unauthorized();

        var result = await mediator.Send(new GetUserByIdQuery(id), ct);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(result, "Current user retrieved"));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser(
        long id, [FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        if (!IsSelfOrAdmin(id))
            return Forbid();

        var result = await mediator.Send(new UpdateUserCommand(id, request), ct);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(result, "Profile updated successfully"));
    }

    [HttpPut("{id:long}/favorite-genres")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateFavoriteGenres(
        long id, [FromBody] UpdateFavoriteGenresRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var currentId) || currentId != id)
            return Forbid();

        var result = await mediator.Send(new UpdateFavoriteGenresCommand(id, request), ct);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(result, "Favorite genres updated"));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse>> DeleteUser(long id, CancellationToken ct)
    {
        if (!IsSelfOrAdmin(id))
            return Forbid();

        await mediator.Send(new DeleteUserCommand(id), ct);
        return Ok(ApiResponse.SuccessResponse("User account deleted successfully"));
    }
}