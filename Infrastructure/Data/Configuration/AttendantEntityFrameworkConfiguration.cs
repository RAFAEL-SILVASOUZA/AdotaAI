using AdotaAI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdotaAI.Infrastructure.Data.Configuration;

public class AttendantEntityFrameworkConfiguration : IEntityTypeConfiguration<Attendant>
{
    public void Configure(EntityTypeBuilder<Attendant> builder)
    {
        builder.ToTable("Attendants");

        builder.Property(a => a.InstitutionId)
            .IsRequired();

        builder.HasOne<Institution>()
            .WithMany()
            .HasForeignKey(a => a.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}