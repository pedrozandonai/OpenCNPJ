using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.SpecialSituations.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class SpecialSituationConfiguration : IEntityTypeConfiguration<SpecialSituation>
{
    public void Configure(EntityTypeBuilder<SpecialSituation> builder)
    {
        builder.ToTable("special_situations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired();
    }
}