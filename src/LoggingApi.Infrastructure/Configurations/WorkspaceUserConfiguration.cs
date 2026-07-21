using LoggingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoggingApi.Infrastructure.Configurations;

/// <summary>
/// Configures persistence for <see cref="WorkspaceUser"/>.
/// </summary>
public sealed class WorkspaceUserConfiguration : IEntityTypeConfiguration<WorkspaceUser>
{
    public void Configure(EntityTypeBuilder<WorkspaceUser> builder)
    {
        builder
            .HasKey(x => new { x.WorkspaceId, x.UserId });
        
        builder
            .HasOne(x => x.Workspace)
            .WithMany(x => x.WorkspaceUsers)
            .HasForeignKey(x => x.WorkspaceId)
            .IsRequired();
        
        builder
            .HasOne(x => x.User)
            .WithMany(x => x.WorkspaceUsers)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
        
        builder
            .HasIndex(x => new { x.WorkspaceId, x.UserId })
            .IsUnique();

        builder
            .Property(x => x.Role)
            .IsRequired();
    }
}