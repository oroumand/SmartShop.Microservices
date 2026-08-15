using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace SmartShop.Loyalty.Infra.Data.Inbox;

public sealed class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
    {
        builder.ToTable("ProcessedMessages");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Type).HasMaxLength(500).IsRequired();
        builder.Property(message => message.ProcessedAtUtc).IsRequired();
    }
}
