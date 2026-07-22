using Pingr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pingr.Infrastructure.Configurations;

/// <summary>
/// Configures persistence settings for the <c>Log</c> entity.
/// </summary>
public sealed class LogConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.Workspace)
            .WithMany(x => x.Logs)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        
        builder
            .HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedLogs)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();
        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x => x.Data)
            .IsRequired()
            .HasColumnType("jsonb");
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(x => x.WorkspaceId);
        builder.HasIndex(x => x.CreatedByUserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Type);
    }
}