using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("batches");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.Identifier)
            .HasColumnName("identifier")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Identifier).IsUnique();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ApplicationLastStep)
            .HasColumnName("application_last_step_id")
            .IsRequired();
    }
}
