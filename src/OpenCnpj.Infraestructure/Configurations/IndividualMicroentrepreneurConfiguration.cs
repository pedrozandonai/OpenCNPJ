using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.IndividualMicroentrepreneurs.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class IndividualMicroentrepreneurConfiguration : IEntityTypeConfiguration<IndividualMicroentrepreneur>
{
    public void Configure(EntityTypeBuilder<IndividualMicroentrepreneur> builder)
    {
        builder.ToTable("individual_microentrepreneurs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.DateOpted)
            .HasColumnName("date_opted")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(x => x.ExclusionDate)
            .HasColumnName("exclusion_date")
            .HasColumnType("date")
            .IsRequired(false);
    }
}