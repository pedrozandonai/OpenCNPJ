using OpenCnpj.Application.Companies.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class CompanyRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), ICompanyRepository
{
    public async Task Insert(IEnumerable<Company> companies, CancellationToken cancellationToken)
    {
        foreach(var company in companies)
        {
            if (company.Country != null)
                openCnpjDbContext.Attach(company.Country);

            if (company.MainEconomicActivity != null)
                openCnpjDbContext.Attach(company.MainEconomicActivity);

            if (company.LegalNature != null)
                openCnpjDbContext.Attach(company.LegalNature);

            if (company.MainPartnerQualification != null)
                openCnpjDbContext.Attach(company.MainPartnerQualification);
        }

        await openCnpjDbContext.AddRangeAsync(companies, cancellationToken);
    }
}
