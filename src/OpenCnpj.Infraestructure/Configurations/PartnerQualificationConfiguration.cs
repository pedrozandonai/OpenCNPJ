using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.PartnersQualifications.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class PartnerQualificationConfiguration : IEntityTypeConfiguration<PartnerQualification>
{
    public void Configure(EntityTypeBuilder<PartnerQualification> builder)
    {
        builder.ToTable("partner_qualifications");

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