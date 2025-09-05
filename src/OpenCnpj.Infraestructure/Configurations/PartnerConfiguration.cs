using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.Partners.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("partners");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey("CompanyId")
            .HasConstraintName("FK_partners_company")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LegalRepresentative)
            .WithMany()
            .HasForeignKey("LegalRepresentativeId")
            .HasConstraintName("FK_partners_legal_representative")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(x => x.PartnerQualification)
            .WithMany()
            .HasForeignKey("PartnerQualificationId")
            .HasConstraintName("FK_partners_partner_qualification")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey("CountryId")
            .HasConstraintName("FK_partners_country")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.Property(x => x.PartnerType)
            .HasColumnName("partner_type_id")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.AgeRange)
            .HasColumnName("age_range_id")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(x => x.Identifier)
            .HasColumnName("identifier")
            .IsRequired(false);

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property<long>("CompanyId").HasColumnName("company_id");
        builder.Property<long?>("LegalRepresentativeId").HasColumnName("legal_representative_id");
        builder.Property<long>("PartnerQualificationId").HasColumnName("partner_qualification_id");
        builder.Property<long?>("CountryId").HasColumnName("country_id");
    }
}
