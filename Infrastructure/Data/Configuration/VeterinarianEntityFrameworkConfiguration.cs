using AdotaAI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdotaAI.Infrastructure.Data.Configuration;

public class VeterinarianEntityFrameworkConfiguration : IEntityTypeConfiguration<Veterinarian>
{
    public void Configure(EntityTypeBuilder<Veterinarian> builder)
    {
        builder.ToTable("Veterinarians");

        builder.Property(v => v.Crmv)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.InstitutionId)
            .IsRequired();

        builder.HasIndex(v => v.Crmv).IsUnique();

        builder.HasOne<Institution>()
            .WithMany()
            .HasForeignKey(v => v.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}