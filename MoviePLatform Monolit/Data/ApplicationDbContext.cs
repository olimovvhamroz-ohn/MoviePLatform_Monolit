using Microsoft.EntityFrameworkCore;
using MoviePlatform_Monolit.Engagment.Entity;
using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<UserFavoriteCategoryEntity> UserFavoriteCategories => Set<UserFavoriteCategoryEntity>();
    
    public DbSet<MovieEntity> Movies => Set<MovieEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<StudioEntity> Studios => Set<StudioEntity>();
    public DbSet<ActorEntity> Actors => Set<ActorEntity>();

    public DbSet<CartEntity> Carts => Set<CartEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();
    public DbSet<PurchaseEntity> Purchases => Set<PurchaseEntity>();

    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();
    public DbSet<ViewEntity> Views => Set<ViewEntity>();
    public DbSet<WatchlistEntity> Watchlists => Set<WatchlistEntity>();

    protected override  void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
}
