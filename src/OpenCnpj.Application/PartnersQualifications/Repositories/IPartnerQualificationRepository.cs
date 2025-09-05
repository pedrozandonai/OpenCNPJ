using OpenCnpj.Application.PartnersQualifications.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.PartnersQualifications.Repositories;
public interface IPartnerQualificationRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(PartnerQualification partnerQualification, CancellationToken cancellationToken);
    Task<PartnerQualification?> GetByCode(long code, CancellationToken cancellationToken);
    Task<IEnumerable<PartnerQualification>> GetAll(CancellationToken cancellationToken);
}