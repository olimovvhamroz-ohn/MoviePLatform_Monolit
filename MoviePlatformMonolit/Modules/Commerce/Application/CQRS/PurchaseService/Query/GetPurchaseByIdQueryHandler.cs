using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Response;

namespace MoviePlatformMonolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

public record GetPurchaseByIdQuery(long UserId, long PurchaseId) : IRequest<PurchaseResponse>;

public class GetPurchaseByIdQueryHandler(ApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetPurchaseByIdQuery, PurchaseResponse>
{
    public async Task<PurchaseResponse> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        var purchase = await context.Purchases
                           .AsNoTracking()
                           .FirstOrDefaultAsync(x => x.Id == request.PurchaseId && x.UserId == request.UserId, cancellationToken)
                       ?? throw new NotFoundException("Purchase ID not Found");

        return mapper.Map<PurchaseResponse>(purchase);
    }
}