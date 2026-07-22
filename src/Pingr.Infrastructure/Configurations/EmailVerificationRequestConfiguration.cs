using Pingr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pingr.Infrastructure.Configurations;

/// <summary>
/// Configures persistence for <see cref="EmailVerificationRequest"/>.
/// </summary>
public class EmailVerificationRequestConfiguration
    : IEntityTypeConfiguration<EmailVerificationRequest>
{
    public void Configure(
        EntityTypeBuilder<EmailVerificationRequest> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<EmailVerificationRequest>(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.ExpiresAt)
            .IsRequired();
        builder.HasIndex(x => x.ExpiresAt);
    }
}