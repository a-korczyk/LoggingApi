using LoggingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoggingApi.Infrastructure.Configurations;

/// <remarks>
/// Sets up the <c>Id</c> field as the primary key, adds an index to the <c>Email</c> field.
/// </remarks>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);
        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();
        
        builder.Property(x => x.EmailConfirmed)
            .IsRequired();
    }
}