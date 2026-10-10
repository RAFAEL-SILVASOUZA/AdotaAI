using AdotaAI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdotaAI.Infrastructure.Data.Configuration;

public class UserEntityFrameworkConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedOnAdd();

        builder.Property(user => user.Name).IsRequired().HasMaxLength(70);
        builder.Property(user => user.Email).IsRequired().HasMaxLength(100);
        builder.Property(user => user.Phone).IsRequired().HasMaxLength(15);
        builder.Property(user => user.Password).IsRequired().HasMaxLength(25);
        builder.Property(user => user.Address).IsRequired().HasMaxLength(100);
        builder.Property(user => user.Photo).IsRequired();

        // Enum Sex: guardado como int (Male = 1, Female = 2, Other = 3).
        builder.Property(user => user.Gender).IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();
    }
}
