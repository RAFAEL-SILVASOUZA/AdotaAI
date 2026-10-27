using AdotaAI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdotaAI.Infrastructure.Data.Configuration;

public class InstitutionEntityFrameworkConfiguration : IEntityTypeConfiguration<Institution>
{
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder.ToTable("Institutions");

        builder.HasKey(institution => institution.Id);
        builder.Property(institution => institution.Id).ValueGeneratedOnAdd();

        builder.Property(institution => institution.Name).IsRequired().HasMaxLength(70);
        builder.Property(institution => institution.Email).IsRequired().HasMaxLength(100);
        builder.Property(institution => institution.Phone).IsRequired().HasMaxLength(15);
        builder.Property(institution => institution.Password).IsRequired().HasMaxLength(25);
        builder.Property(institution => institution.Address).IsRequired().HasMaxLength(100);
        builder.Property(institution => institution.Photo).IsRequired();
        builder.Property(institution => institution.Document).IsRequired().HasMaxLength(18);
        builder.Property(institution => institution.Description).IsRequired().HasMaxLength(500);
        builder.Property(institution => institution.OperatingHours).IsRequired().HasMaxLength(100);


        builder.HasIndex(institution => institution.Email).IsUnique();
    }
}
