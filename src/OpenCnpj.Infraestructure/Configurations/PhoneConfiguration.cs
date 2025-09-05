using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.Phones.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.ToTable("phones");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.AreaCode)
            .HasColumnName("area_code")
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasColumnName("phone")
            .IsRequired();
    }
}