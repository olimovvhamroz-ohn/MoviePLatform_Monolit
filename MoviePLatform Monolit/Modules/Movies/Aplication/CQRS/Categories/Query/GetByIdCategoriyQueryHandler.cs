using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;
using UserService.Domain.Extensions;

namespace MoviePLatform_Monolit.Movie.CQRS.Categories.Query;


public record GetCategoryByIdQuery(int Id) : IRequest<CategoryResponse>;


public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly ICategoriyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;
    private readonly IDistributedCache _cache;

    public GetCategoryByIdQueryHandler(
        ILogger<GetCategoryByIdQueryHandler> logger, 
        ICategoriyRepository repository, 
        IMapper mapper,IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        
        var key=$"category:{request.Id}";
        var cach=await _cache.GetStringAsync(key,cancellationToken);
        if (!string.IsNullOrEmpty(cach))
        {
            _logger.LogInformation("Category found with id: {Id}", request.Id);
            return JsonSerializer.Deserialize<CategoryResponse>(cach)!;
        }
        
        _logger.LogInformation("Getting category by id: {Id} via CQRS", request.Id);
        
        var data = await _repository.GetByIdAsync(request.Id,cancellationToken);
        if (data == null)
        {
            _logger.LogWarning("Category not found with id: {Id}", request.Id);
            throw new NotFoundException("Category not found");
        }

        _logger.LogInformation("Category found: {Title}", data.Title);

       var response = _mapper.Map<CategoryResponse>(data);
       var json = JsonSerializer.Serialize(response);
       
       await _cache.SetStringAsync(
           key,
           json,
           new DistributedCacheEntryOptions
            {
           AbsoluteExpirationRelativeToNow = 
               TimeSpan.FromMinutes(10)
            }, 
           cancellationToken);
       _logger.LogInformation("Category {Id} loaded",request.Id);
       return response;
    }
}