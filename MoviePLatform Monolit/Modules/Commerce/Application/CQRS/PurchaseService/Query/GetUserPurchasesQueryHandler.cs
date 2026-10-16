using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Users.Application.Policies;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

public record GetUserPurchasesQuery(long UserId) : IRequest<List<PurchaseResponse>>;

public class GetUserPurchasesQueryHandler(IPurchaseRepository repository, IMapper mapper) : IRequestHandler<GetUserPurchasesQuery, List<PurchaseResponse>>
{
    
    public async Task<List<PurchaseResponse>> Handle(
        GetUserPurchasesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await repository.GetUserPurchases(request.UserId);

        
        var result = mapper.Map<List<PurchaseResponse>>(data);

        for (int i = 0; i < data.Count; i++)
        {
            result[i].RentalStartDeadline =
                RentalPolicy.GetStartDeadline(data[i].PurchasedAt);

            result[i].ExpiresAt =
                RentalPolicy.GetExpiresAt(data[i].FirstWatchedAt);
        }

        return result;
    }
}