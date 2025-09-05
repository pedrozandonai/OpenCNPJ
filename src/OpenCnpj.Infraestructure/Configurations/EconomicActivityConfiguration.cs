using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.EconomicActivities.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class EconomicActivityConfiguration : IEntityTypeConfiguration<EconomicActivity>
{
    public void Configure(EntityTypeBuilder<EconomicActivity> builder)
    {
        builder.ToTable("economic_activities");

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
