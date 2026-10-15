using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

public record GetPurchaseByIdQuery(long UserId, long PurchaseId) : IRequest<PurchaseResponse>;

public class GetPurchaseByIdQueryHandler(IPurchaseRepository repository,IMapper mapper):IRequestHandler<GetPurchaseByIdQuery,PurchaseResponse>
{
    
    public async Task<PurchaseResponse> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetPurchaseById(request.PurchaseId, request.UserId);
        if (result == null)
        {
            throw new NotFoundException("Purchase ID not Found");
        }; 
        return mapper.Map<PurchaseResponse>(result);
         
    }
}