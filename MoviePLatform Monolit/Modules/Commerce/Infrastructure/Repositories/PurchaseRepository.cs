using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Commerce.Infrastructure.Repositories;

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

    public async Task<List<PurchaseEntity>> GetAllPurchases(int page, int pageSize,CancellationToken cancellationToken = default)
    {
        if(page<=0)page=1;
        if(pageSize<=0)pageSize=10;
        
        var skip =(page-1)*pageSize;
        
        return await context.Purchases
        .AsNoTracking()
        .Include(x=>x.User)
        .Include(x=>x.Movie)
        .AsSplitQuery()
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
        
    }
}