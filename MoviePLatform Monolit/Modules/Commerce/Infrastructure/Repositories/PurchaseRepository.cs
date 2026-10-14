

using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Interfaces;

public class PurchaseRepository(ApplicationDbContext context) : IPurchaseRepository
{
    

    public async Task<bool> HasUserPurchasedMovie(long userId, long movieId)
    {
        return await context.Purchases
            .AnyAsync(x => x.UserId == userId && x.MovieId == movieId);
    }

    public async Task<PurchaseEntity> CreatePurchase(PurchaseEntity create)
    {
        await context.Purchases.AddAsync(create);
        await context.SaveChangesAsync();
        return create;
    }

    public async Task<List<PurchaseEntity>> GetUserPurchases(long id)
    {
        return await context.Purchases
            .Where(x => x.UserId == id)
            .ToListAsync();
    }
    public async Task<PurchaseEntity?> GetByUserAndMovie(long userId, long movieId)
    {
        return await context.Purchases
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);
    }

    public async Task<PurchaseEntity?> GetPurchaseById(long purchaseId,long userId)
    {
        return await context.Purchases.FirstOrDefaultAsync(x=>x.Id == purchaseId && x.UserId ==userId);
    }
    
    
    public async Task<PurchaseEntity> UpdatePurchase(PurchaseEntity entity)
    {
        context.Purchases.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}