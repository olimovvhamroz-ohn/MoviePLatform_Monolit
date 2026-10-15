using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Command.Interfaces;

public interface IBaseRepo<T>
{
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> GetByIdAsync(long id,CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity,CancellationToken cancellationToken = default);
    Task<T>UpdateAsync(T entity,CancellationToken cancellationToken = default);
    Task<T> DeleteAsync(long id,CancellationToken cancellationToken = default);
}
public class BaseRepository<T>(ApplicationDbContext context):IBaseRepo<T> where T:BaseEntity
{
    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }
    public async Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new BadRequestException("ID must be greater than zero");
        
        var res = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if (res == null)
            throw new NotFoundException($"Id {id} not found");
        
        return res;
    }

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) throw new BadRequestException("Entity can not be null");
        await context.Set<T>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) throw new BadRequestException("Entity can not be null");
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return entity;
    }

    public async Task<T> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new BadRequestException("ID must be greater than zero");
        var entity = await GetByIdAsync(id, cancellationToken);
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}