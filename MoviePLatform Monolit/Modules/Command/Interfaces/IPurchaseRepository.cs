

using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Modules.Command.Interfaces;

public interface IPurchaseRepository
{
    Task<bool> HasUserPurchasedMovie(long userId, long movieId);
    Task<PurchaseEntity> CreatePurchase(PurchaseEntity create);
    Task<PurchaseEntity?> GetByUserAndMovie(long userId, long movieId);
    Task<PurchaseEntity> UpdatePurchase(PurchaseEntity entity);
    Task<List<PurchaseEntity>> GetUserPurchases(long id);
    Task<PurchaseEntity?> GetPurchaseById(long purchaseId,long userId);
}