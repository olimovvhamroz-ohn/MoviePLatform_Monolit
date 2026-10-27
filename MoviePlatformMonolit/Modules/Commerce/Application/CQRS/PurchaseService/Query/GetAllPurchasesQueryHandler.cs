using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Response;

namespace MoviePlatformMonolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

public record GetAllPurchasesQuery(int Page, int PageSize) : IRequest<List<PurchaseResponse>>;

public class GetAllPurchasesQueryHandler(
    ApplicationDbContext context,
    ILogger<GetAllPurchasesQueryHandler> logger,
    IMapper mapper) : IRequestHandler<GetAllPurchasesQuery, List<PurchaseResponse>>
{
    private const int MaxPageSize = 100;

    public async Task<List<PurchaseResponse>> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            throw new BadRequestException("Page and PageSize must be greater than 0");

        var pageSize = Math.Min(request.PageSize, MaxPageSize);
        var skip = (request.Page - 1) * pageSize;

        logger.LogInformation("Getting purchase page {Page} size {PageSize}", request.Page, pageSize);

        var purchases = await context.Purchases
            .AsNoTracking()
            .OrderByDescending(x => x.PurchasedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<PurchaseResponse>>(purchases);
    }
}