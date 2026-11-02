using AdotaAI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdotaAI.Infrastructure.Data.Configuration;

public class AdmEntityFrameworkConfiguration : IEntityTypeConfiguration<Adm>
{
    public void Configure(EntityTypeBuilder<Adm> builder)
    {
        builder.ToTable("Adms");
    }
}