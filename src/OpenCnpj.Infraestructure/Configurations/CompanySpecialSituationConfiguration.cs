using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.CompanySpecialSituations.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class CompanySpecialSituationConfiguration : IEntityTypeConfiguration<CompanySpecialSituation>
{
    public void Configure(EntityTypeBuilder<CompanySpecialSituation> builder)
    {
        builder.ToTable("company_special_situations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("date")
            .IsRequired();

        builder.HasOne(x => x.SpecialSituation)
            .WithMany()
            .HasForeignKey("SpecialSituationId")
            .HasConstraintName("FK_company_special_situations_special_situations")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property<int>("SpecialSituationId").HasColumnName("special_situation_id");
    }
}
