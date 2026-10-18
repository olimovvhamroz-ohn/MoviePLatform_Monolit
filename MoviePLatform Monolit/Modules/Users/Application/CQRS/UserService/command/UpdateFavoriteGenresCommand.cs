using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.UserService.command;

public record UpdateFavoriteGenresCommand(long UserId, UpdateFavoriteGenresRequest Dto) : IRequest<UserResponse>;

public class UpdateFavoriteGenresCommandHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<UpdateFavoriteGenresCommandHandler> logger) : IRequestHandler<UpdateFavoriteGenresCommand, UserResponse>
{
    public async Task<UserResponse> Handle(UpdateFavoriteGenresCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found");

        var categoryIds = request.Dto.CategoryIds.Distinct().ToList();

        var existingIds = await context.Categories
            .Where(x => categoryIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missing = categoryIds.Except(existingIds).ToList();
        if (missing.Count > 0)
            throw new BadRequestException($"Categories not found: {string.Join(",", missing)}");

        var old = await context.UserFavoriteCategories
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        context.UserFavoriteCategories.RemoveRange(old);
        context.UserFavoriteCategories.AddRange(categoryIds.Select(id => new UserFavoriteCategoryEntity
        {
            UserId = request.UserId,
            CategoryId = id
        }));

        // Удаление и добавление уходят одной транзакцией
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} updated favorite genres: {Ids}",
            request.UserId, string.Join(",", categoryIds));

        return mapper.Map<UserResponse>(user);
    }
}