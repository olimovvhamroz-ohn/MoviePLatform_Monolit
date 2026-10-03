
    using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Commerce.Infrastructure.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable("orders", "commerce");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalPrice).HasColumnType("numeric(10,2)");
        builder.Property(o => o.Notes).HasMaxLength(500);

        builder.HasIndex(o => o.UserId);

        // Як заказ → якчанд элемент
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Як заказ → як платёж
        builder.HasOne(o => o.Payment)
            .WithOne(p => p.Order)
            .HasForeignKey<PaymentEntity>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
    {
        builder.ToTable("order_items", "commerce");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Price).HasColumnType("numeric(10,2)");

        builder.HasIndex(i => i.OrderId);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(EntityTypeBuilder<PaymentEntity> builder)
    {
        builder.ToTable("payments", "commerce");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("numeric(10,2)");
        builder.Property(p => p.TransactionId).HasMaxLength(100);

        // TransactionId-и шлюз такрор намешавад (агар пур бошад)
        builder.HasIndex(p => p.TransactionId)
            .IsUnique()
            .HasFilter("\"TransactionId\" IS NOT NULL");
    }
}
