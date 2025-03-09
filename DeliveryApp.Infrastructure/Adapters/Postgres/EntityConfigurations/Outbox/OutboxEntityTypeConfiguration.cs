using DeliveryApp.Infrastructure.Adapters.Postgres.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasketApp.Infrastructure.Adapters.Postgres.EntityConfigurations.Outbox;

internal class OutboxEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> entityTypeBuilder)
    {
        entityTypeBuilder
            .ToTable("outbox");

        entityTypeBuilder
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        entityTypeBuilder
            .Property(entity => entity.Type)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("type")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.Content)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("content")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.OccuredOnUtc)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("occured_on_utc")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.ProcessedOnUtc)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("processed_on_utc")
            .IsRequired(false);
    }
}