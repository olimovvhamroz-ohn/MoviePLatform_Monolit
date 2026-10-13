

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public record GetUserPurchasesQuery(int UserId) : IRequest<List<PurchaseResponse>>;

public class GetUserPurchasesQueryHandler : IRequestHandler<GetUserPurchasesQuery, List<PurchaseResponse>>
{
    private readonly IPurchaseRepository _repository;
    private readonly IMapper _mapper;

    public GetUserPurchasesQueryHandler(IPurchaseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<List<PurchaseResponse>> Handle(
        GetUserPurchasesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _repository.GetUserPurchases(request.UserId);

        
        var result = _mapper.Map<List<PurchaseResponse>>(data);

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