using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.LegalNatures.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class LegalNatureConfiguration : IEntityTypeConfiguration<LegalNature>
{
    public void Configure(EntityTypeBuilder<LegalNature> builder)
    {
        builder.ToTable("legal_natures");

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
