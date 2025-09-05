using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.Companies.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("company");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.HasOne(x => x.LegalNature)
            .WithMany()
            .HasForeignKey("LegalNatureId")
            .HasConstraintName("FK_companies_legal_natures")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(x => x.MainPartnerQualification)
            .WithMany()
            .HasForeignKey("MainPartnerQualificationId")
            .HasConstraintName("FK_companies_partner_qualifications")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.Property(x => x.CompanySize)
            .HasColumnName("company_size_id")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CompanyType)
            .HasColumnName("company_type_id")
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey("CountryId")
            .HasConstraintName("FK_companies_countries")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Address)
            .WithMany()
            .HasForeignKey("AddressId")
            .HasConstraintName("FK_companies_addresses")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired();

        builder.HasOne(x => x.CompanySpecialSituation)
            .WithMany()
            .HasForeignKey("CompanySpecialSituationId")
            .HasConstraintName("FK_companies_companies_special_situations")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.MainEconomicActivity)
            .WithMany()
            .HasForeignKey("MainEconomicActivityId")
            .HasConstraintName("FK_companies_economic_activities")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired();

        builder.Property(x => x.Identifier)
            .HasColumnName("identifier")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(x => x.ShareCapital)
            .HasColumnName("share_capital")
            .HasColumnType("decimal")
            .IsRequired();

        builder.Property(x => x.ResponsableFederativeEntity)
            .HasColumnName("responsabile_federative_entity")
            .IsRequired(false);

        builder.Property(x => x.FantasyName)
            .HasColumnName("fantasy_name")
            .IsRequired(false);

        builder.Property(x => x.RegisterDate)
            .HasColumnName("register_date")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(x => x.ForeingCityName)
            .HasColumnName("foreign_city_name")
            .IsRequired(false);

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property<long>("LegalNatureId").HasColumnName("legal_nature_id");
        builder.Property<long?>("MainPartnerQualificationId").HasColumnName("main_partner_qualification_id");
        builder.Property<long?>("CountryId").HasColumnName("country_id");
        builder.Property<long>("AddressId").HasColumnName("address_id");
        builder.Property<long?>("CompanySpecialSituationId").HasColumnName("company_special_situation_id");
        builder.Property<long?>("MainEconomicActivityId").HasColumnName("main_economic_activity_id");
    }
}
