

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record CreatCartCommand(long userId):IRequest<CartResponse>;
public class CreatCartCommandHandler:IRequestHandler<CreatCartCommand,CartResponse>

{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreatCartCommandHandler> _logger;

    public CreatCartCommandHandler(ICartRepository cartRepository, ILogger<CreatCartCommandHandler> logger, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<CartResponse> Handle(CreatCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating cart for user {UserId}", request.userId);

        if (request.userId <= 0)
            throw new BadRequestException("Invalid userId");

        var res = await _cartRepository.CreateCart(request.userId);
        return _mapper.Map<CartResponse>(res);
    }
    
}