using Microsoft.EntityFrameworkCore;
using UserService.Domain.Extensions;

namespace MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

public interface IBaseRepo<T>
{
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> GetByIdAsync(long id,CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity,CancellationToken cancellationToken = default);
    Task<T>UpdateAsync(T entity,CancellationToken cancellationToken = default);
    Task<T> DeleteAsync(long id,CancellationToken cancellationToken = default);
}
public class BaseRepository<T>:IBaseRepo<T> where T:BaseEntity
{
    protected readonly ApplicationDbContext _context;
    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T> GetByIdAsync(long id,CancellationToken cancellationToken = default)
    {
        if(id<=0) throw new ArgumentException("ID must be greater than zero.", nameof(id));
        var res = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
         if (res == null)
         throw new NotFoundException($"Id {id} not found");       
         return res; 
    }

    public async Task<T> CreateAsync(T entity,CancellationToken cancellationToken=default)
    {
        if (entity==null) throw new Exception("BadrequestEx Entitiy can not be null");
        await _context.Set<T>().AddAsync(entity,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
        
        
    }

    public async Task<T> UpdateAsync(T entity,CancellationToken cancellationToken = default)
    {
        if(entity==null) throw new Exception("BadrequestEx= entity cannot be null");
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<T> DeleteAsync(long id,CancellationToken cancellationToken = default)
    {
        if(id<=0)throw new Exception("BadRequest : id can not 'id>=0'");
        var entity=await GetByIdAsync(id,cancellationToken);
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}