using AdotaAI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdotaAI.Infrastructure.Data.Configuration;

public class PetEntityFrameworkConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");

        builder.HasKey(pet => pet.Id);
        builder.Property(pet => pet.Id).ValueGeneratedOnAdd();

        builder.Property(pet => pet.Name).IsRequired().HasMaxLength(70);
        builder.Property(pet => pet.Species).IsRequired().HasMaxLength(50);
        builder.Property(pet => pet.Race).IsRequired().HasMaxLength(50);
        builder.Property(pet => pet.Age).IsRequired();
        builder.Property(pet => pet.IsFemale).IsRequired();
        builder.Property(pet => pet.PetSize).IsRequired();
        builder.Property(pet => pet.BehaviourDesc).IsRequired().HasMaxLength(500);
        builder.Property(pet => pet.Photo).IsRequired();
        builder.Property(pet => pet.PetStatus).IsRequired();
        builder.Property(pet => pet.VetRecord).IsRequired().HasMaxLength(500);
        builder.Property(pet => pet.InstitutionId).IsRequired();
    }
}
