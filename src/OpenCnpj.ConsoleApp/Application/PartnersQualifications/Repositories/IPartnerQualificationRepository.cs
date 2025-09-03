using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.PartnersQualifications.Repositories;
public interface IPartnerQualificationRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(PartnerQualification partnerQualification, CancellationToken cancellationToken);
    Task<PartnerQualification?> GetByCode(string code, CancellationToken cancellationToken);
}