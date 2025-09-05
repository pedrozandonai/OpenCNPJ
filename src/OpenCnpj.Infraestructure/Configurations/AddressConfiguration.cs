using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.Addresses.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.HasOne(x => x.AddressType)
            .WithMany()
            .HasForeignKey("AddressTypeId")
            .HasConstraintName("FK_addresses_address_types")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.City)
            .WithMany()
            .HasForeignKey("CityId")
            .HasConstraintName("FK_addresses_cities")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Street)
            .HasColumnName("street")
            .IsRequired();

        builder.Property(x => x.Number)
            .HasColumnName("number")
            .IsRequired(false);

        builder.Property(x => x.Complement)
            .HasColumnName("complement")
            .IsRequired(false);

        builder.Property(x => x.Neighborhood)
            .HasColumnName("neighborhood")
            .IsRequired();

        builder.Property(x => x.ZipCode)
            .HasColumnName("zip_code")
            .IsRequired(false);

        builder.Property(x => x.FederalUnit)
            .HasColumnName("federal_unit")
            .IsRequired();

        builder.Property<int>("AddressTypeId").HasColumnName("address_type_id");
        builder.Property<long?>("CityId").HasColumnName("city_id");
    }
}
