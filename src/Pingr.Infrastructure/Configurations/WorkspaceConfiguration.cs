using Pingr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pingr.Infrastructure.Configurations;

/// <summary>
/// Configures persistence for <see cref="Workspace"/>.
/// </summary>
public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.OwnerUser)
            .WithMany(x => x.OwnedWorkspaces)
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder
            .Property(x => x.Name)
            .IsRequired();
        builder
            .HasIndex(x => x.Name);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}