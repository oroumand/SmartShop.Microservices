using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartShop.Payments.Infra.Data.Outbox;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", "payments");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Type).HasMaxLength(500).IsRequired();
        builder.Property(message => message.RoutingKey).HasMaxLength(200).IsRequired();
        builder.Property(message => message.Payload).IsRequired();
        builder.Property(message => message.LastError).HasMaxLength(2000);
        builder.HasIndex(message => new { message.ProcessedAtUtc, message.CreatedAtUtc });
    }
}
