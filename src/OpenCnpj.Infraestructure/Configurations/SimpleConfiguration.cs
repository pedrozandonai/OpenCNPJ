//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using OpenCnpj.Application.Simples.Domain;

//namespace OpenCnpj.Infraestructure.Configurations;
//public class SimpleConfiguration : IEntityTypeConfiguration<Simple>
//{
//    public void Configure(EntityTypeBuilder<Simple> builder)
//    {
//        builder.ToTable("simples");

//        builder.HasKey(x => x.Id);
//        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

//        builder.Property(x => x.CompanyId)
//            .HasColumnName("company_id")
//            .IsRequired();

//        builder.Property(x => x.MeiId)
//            .HasColumnName("mei_id")
//            .IsRequired(false);

//        builder.Property(x => x.IsSimple)
//            .HasColumnName("is_simple")
//            .IsRequired(false);

//        builder.Property(x => x.DateOpted)
//            .HasColumnName("date_opted")
//            .HasColumnType("date")
//            .IsRequired(false);

//        builder.Property(x => x.ExclusionDate)
//            .HasColumnName("exclusion_date")
//            .HasColumnType("date")
//            .IsRequired(false);

//        // Foreign Keys
//        builder.HasOne(x => x.Company)
//            .WithMany()
//            .HasForeignKey(x => x.CompanyId)
//            .OnDelete(DeleteBehavior.Cascade);

//        builder.HasOne(x => x.Mei)
//            .WithMany()
//            .HasForeignKey(x => x.MeiId)
//            .OnDelete(DeleteBehavior.SetNull);
//    }
//}
