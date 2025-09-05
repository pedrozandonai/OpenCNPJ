using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.Cities.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("cities");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.Code)
            .HasColumnName("code");

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired();
    }
}