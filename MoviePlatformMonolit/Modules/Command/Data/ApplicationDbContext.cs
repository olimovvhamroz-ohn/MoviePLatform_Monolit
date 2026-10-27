using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Commerce.Domain.Entity;
using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;
using MoviePlatformMonolit.Modules.Users.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Command.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<UserFavoriteCategoryEntity> UserFavoriteCategories => Set<UserFavoriteCategoryEntity>();

    public DbSet<MovieEntity> Movies => Set<MovieEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<StudioEntity> Studios => Set<StudioEntity>();
    public DbSet<ActorEntity> Actors => Set<ActorEntity>();

    public DbSet<CartEntity> Carts => Set<CartEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();
    public DbSet<PurchaseEntity> Purchases => Set<PurchaseEntity>();
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();
    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();
    public DbSet<ViewEntity> Views => Set<ViewEntity>();
    public DbSet<WatchListEntity> WatchLists => Set<WatchListEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<MovieEntity>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);
        modelBuilder.ApplySoftDeleteFilters();
    }
}