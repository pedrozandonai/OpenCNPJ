using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Application.AddressTypes.Domain;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Cities.Domain;
using OpenCnpj.Application.Companies.Domain;
using OpenCnpj.Application.CompanySpecialSituations.Domain;
using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Application.PartnersQualifications.Domain;
using OpenCnpj.Application.Reasons.Domain;
using OpenCnpj.Application.SpecialSituations.Domain;
using OpenCnpj.Core.Interfaces;
using OpenCnpj.Infraestructure.Extensions;
using System.Reflection;

namespace OpenCnpj.Infraestructure.DbContexts;

public class OpenCnpjDbContext(DbContextOptions<OpenCnpjDbContext> options) : DbContext(options), IDbContext
{
    public DbSet<Batch> Batches { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<EconomicActivity> EconomicActivities{ get; set; }
    public DbSet<LegalNature> LegalNatures { get; set; }
    public DbSet<PartnerQualification> PartnerQualifications { get; set; }
    public DbSet<Reason> Reasons { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<AddressType> AddressTypes { get; set; }
    public DbSet<SpecialSituation> SpecialSituations { get; set; }
    public DbSet<CompanySpecialSituation> CompanySpecialSituations { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ConfigureEnums();

        base.OnModelCreating(modelBuilder);
    }

    public Task TestConnection()
    {
        throw new NotImplementedException();
    }
}

