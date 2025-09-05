using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.Batches.BatchFiles.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
public class BatchFileConfiguration : IEntityTypeConfiguration<BatchFile>
{
    public void Configure(EntityTypeBuilder<BatchFile> builder)
    {
        builder.ToTable("batch_files");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.BatchId)
            .HasColumnName("batch_id")
            .IsRequired();

        builder.Property(x => x.FileStatus)
            .HasColumnName("file_status_id")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FilePath)
            .HasColumnName("file_path")
            .HasMaxLength(255)
            .IsRequired();
    }
}
