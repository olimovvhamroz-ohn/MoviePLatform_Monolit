

using MoviePLatform_Monolit.Entity;

public interface IPurchaseRepository
{
    Task<bool> HasUserPurchasedMovie(long userId, long movieId);
    Task<PurchaseEntity> CreatePurchase(PurchaseEntity create);
    Task<PurchaseEntity?> GetByUserAndMovie(long userId, long movieId);
    Task<PurchaseEntity> UpdatePurchase(PurchaseEntity entity);
    Task<List<PurchaseEntity>> GetUserPurchases(int id);
}