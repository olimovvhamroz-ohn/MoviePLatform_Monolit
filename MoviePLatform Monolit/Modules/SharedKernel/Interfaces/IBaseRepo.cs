
public interface IBaseRepo<T>
{
    Task<List<T>> GetAllAsync();
    Task<T> GetById(long id);
    Task<T>CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(long id);
}