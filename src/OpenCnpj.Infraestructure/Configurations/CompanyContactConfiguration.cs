//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using OpenCnpj.Application.CompanyContacts.Domain;

//namespace OpenCnpj.Infraestructure.Configurations;
//public class CompanyContactConfiguration : IEntityTypeConfiguration<CompanyContact>
//{
//    public void Configure(EntityTypeBuilder<CompanyContact> builder)
//    {
//        builder.ToTable("company_contacts");

//        builder.HasKey(x => new { x.CompanyId, x.ContactId });

//        builder.Property(x => x.CompanyId)
//            .HasColumnName("company_id")
//            .IsRequired();

//        builder.Property(x => x.ContactId)
//            .HasColumnName("contact_id")
//            .IsRequired();

//        // Foreign Keys
//        builder.HasOne(x => x.Company)
//            .WithMany(x => x.CompanyContacts)
//            .HasForeignKey(x => x.CompanyId)
//            .OnDelete(DeleteBehavior.Cascade);

//        builder.HasOne(x => x.Contact)
//            .WithMany(x => x.CompanyContacts)
//            .HasForeignKey(x => x.ContactId)
//            .OnDelete(DeleteBehavior.Cascade);
//    }
//}
