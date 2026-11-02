using AdotaAI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdotaAI.Infrastructure.Data.Configuration;

public class EmployeeEntityFrameworkConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(employee => employee.Id);
        builder.Property(employee => employee.Id).ValueGeneratedOnAdd();

        builder.Property(employee => employee.Name).IsRequired().HasMaxLength(70);
        builder.Property(employee => employee.Email).IsRequired().HasMaxLength(100);
        builder.Property(employee => employee.Password).IsRequired().HasMaxLength(25);
        builder.Property(employee => employee.Cpf).IsRequired().HasMaxLength(11).IsFixedLength();

        builder.HasIndex(employee => employee.Email).IsUnique();
        builder.HasIndex(employee => employee.Cpf).IsUnique();
    }
}
