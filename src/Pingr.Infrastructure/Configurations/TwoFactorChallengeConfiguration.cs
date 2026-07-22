using Pingr.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pingr.Infrastructure.Configurations;

/// <summary>
/// Configures persistence for <see cref="TwoFactorChallenge"/>.
/// </summary>
public sealed class TwoFactorChallengeConfiguration : IEntityTypeConfiguration<TwoFactorChallenge>
{
    public void Configure(EntityTypeBuilder<TwoFactorChallenge> builder)
    {
        builder.HasKey(x => x.UserId);
        
        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<TwoFactorChallenge>(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.TwoFactorChallengePurpose)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();
    }
}