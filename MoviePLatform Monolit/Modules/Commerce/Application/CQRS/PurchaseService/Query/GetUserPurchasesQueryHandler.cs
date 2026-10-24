using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.Response;
using MoviePLatform_Monolit.Modules.Users.Application.Policies;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

public record GetUserPurchasesQuery(long UserId) : IRequest<List<PurchaseResponse>>;

public class GetUserPurchasesQueryHandler(ApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetUserPurchasesQuery, List<PurchaseResponse>>
{
    public async Task<List<PurchaseResponse>> Handle(GetUserPurchasesQuery request, CancellationToken cancellationToken)
    {
        var purchases = await context.Purchases
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.PurchasedAt)
            .ToListAsync(cancellationToken);

        return purchases.Select(p =>
        {
            var dto = mapper.Map<PurchaseResponse>(p);
            dto.RentalStartDeadline = RentalPolicy.GetStartDeadline(p.PurchasedAt);
            dto.ExpiresAt = RentalPolicy.GetExpiresAt(p.FirstWatchedAt);
            return dto;
        }).ToList();
    }
}