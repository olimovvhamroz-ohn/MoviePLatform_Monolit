

using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Entity;

public class PurchaseRepository : IPurchaseRepository
{
    protected readonly ApplicationDbContext _context;

    public PurchaseRepository(ApplicationDbContext con)
    {
        _context = con;
    }

    public async Task<bool> HasUserPurchasedMovie(long userId, long movieId)
    {
        return await _context.Purchases
            .AnyAsync(x => x.UserId == userId && x.MovieId == movieId);
    }

    public async Task<PurchaseEntity> CreatePurchase(PurchaseEntity create)
    {
        await _context.Purchases.AddAsync(create);
        await _context.SaveChangesAsync();
        return create;
    }

    public async Task<List<PurchaseEntity>> GetUserPurchases(int id)
    {
        return await _context.Purchases
            .Where(x => x.UserId == id)
            .ToListAsync();
    }
    public async Task<PurchaseEntity?> GetByUserAndMovie(long userId, long movieId)
    {
        return await _context.Purchases
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);
    }
    
    public async Task<PurchaseEntity> UpdatePurchase(PurchaseEntity entity)
    {
        _context.Purchases.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}