using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

public record GetAllPurchasesQuery(int Page, int PageSize) : IRequest<List<PurchaseResponse>>;

public class GetAllPurchasesQueryHandler(IPurchaseRepository repository, ILogger<GetAllPurchasesQueryHandler> logger, IMapper mapper)
    : IRequestHandler<GetAllPurchasesQuery, List<PurchaseResponse>>
{
    public async Task<List<PurchaseResponse>> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException(
                "Page and PageSize must be greater than 0");
        }

        logger.LogInformation(
            "Getting purchase page {Page} size {PageSize} from database",
            request.Page,
            request.PageSize);
        
        var gaetAllPurchase = await repository.GetAllPurchases(request.Page, request.PageSize, cancellationToken);
        
        var gaetA = mapper.Map<List<PurchaseResponse>>(gaetAllPurchase);
        
        return gaetA;
    }
}