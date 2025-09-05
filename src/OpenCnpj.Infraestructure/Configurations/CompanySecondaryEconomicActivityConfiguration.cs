//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Domain;

//namespace OpenCnpj.Infraestructure.Configurations;
//public class CompanySecondaryEconomicActivityConfiguration : IEntityTypeConfiguration<CompanySecondaryEconomicActivity>
//{
//    public void Configure(EntityTypeBuilder<CompanySecondaryEconomicActivity> builder)
//    {
//        builder.ToTable("company_secondary_economic_activities");

//        builder.HasKey(x => new { x.CompanyId, x.EconomicActivityId });

//        builder.Property(x => x.CompanyId)
//            .HasColumnName("company_id")
//            .IsRequired();

//        builder.Property(x => x.EconomicActivityId)
//            .HasColumnName("economic_activity_id")
//            .IsRequired();

//        // Foreign Keys
//        builder.HasOne(x => x.Company)
//            .WithMany(x => x.CompanySecondaryEconomicActivities)
//            .HasForeignKey(x => x.CompanyId)
//            .OnDelete(DeleteBehavior.Cascade);

//        builder.HasOne(x => x.EconomicActivity)
//            .WithMany(x => x.CompanySecondaryEconomicActivities)
//            .HasForeignKey(x => x.EconomicActivityId)
//            .OnDelete(DeleteBehavior.Cascade);
//    }
//}
