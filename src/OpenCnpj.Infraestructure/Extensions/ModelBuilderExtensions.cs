using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.ApplicationSteps.Models.Enums;
using OpenCnpj.Application.Batches.BatchFiles.Models.Enums;
using OpenCnpj.Application.Enums;

namespace OpenCnpj.Infraestructure.Extensions;
public static class ModelBuilderExtensions
{
    public static void ConfigureEnums(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<EApplicationStep>(schema: "public", name: "application_steps");
        modelBuilder.HasPostgresEnum<EFileStatus>(schema: "public", name: "file_statuses");
        modelBuilder.HasPostgresEnum<EAgeRanges>(schema: "public", name: "age_ranges");
        modelBuilder.HasPostgresEnum<ECompanySituation>(schema: "public", name: "company_situations");
        modelBuilder.HasPostgresEnum<ECompanySize>(schema: "public", name: "company_sizes");
        modelBuilder.HasPostgresEnum<ECompanyType>(schema: "public", name: "company_types");
        modelBuilder.HasPostgresEnum<EPartnerType>(schema: "public", name: "partner_types");
    }
}
