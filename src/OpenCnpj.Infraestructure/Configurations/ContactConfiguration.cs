using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCnpj.Application.Contacts.Domain;

namespace OpenCnpj.Infraestructure.Configurations;
//public class ContactConfiguration : IEntityTypeConfiguration<Contact>
//{
    //public void Configure(EntityTypeBuilder<Contact> builder)
    //{
    //    builder.ToTable("contacts");

    //    builder.HasKey(x => x.Id);
    //    builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

    //    builder.Property(x => x.PhoneId)
    //        .HasColumnName("phone_id")
    //        .IsRequired();

    //    builder.Property(x => x.FaxAreaCode)
    //        .HasColumnName("fax_area_code")
    //        .IsRequired();

    //    builder.Property(x => x.FaxNumber)
    //        .HasColumnName("fax_number")
    //        .IsRequired();

    //    builder.Property(x => x.EmailAddress)
    //        .HasColumnName("email_address")
    //        .IsRequired(false);

    //    // Foreign Key
    //    builder.HasOne(x => x.Phone)
    //        .WithMany()
    //        .HasForeignKey(x => x.PhoneId)
    //        .OnDelete(DeleteBehavior.Cascade);
    //}
//}
