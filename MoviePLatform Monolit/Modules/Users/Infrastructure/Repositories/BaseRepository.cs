

using Microsoft.EntityFrameworkCore;
using UserService.Domain.Extensions;

public class BaseRepository<T>:IBaseRepo<T> where T:BaseEntity
{
    protected readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetById(long id)
    {
        var res = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);

        if (res == null) throw new NotFoundException($"Id {id} not found");
        return res;
    }

    public async Task<T> CreateAsync(T entity)
    {
        if(entity==null)throw new BadRequestException("Entity cannot be null");
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
        
    }
    public async Task<T> DeleteAsync(long id)
    {
        var entity = await GetById(id); 
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        if(entity==null)throw new BadRequestException("Entity cannot be null");
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    
    
}