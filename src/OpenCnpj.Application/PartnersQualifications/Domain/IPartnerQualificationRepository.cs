using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.PartnersQualifications.Domain;
public interface IPartnerQualificationRepository : IBaseRepository
{
    Task Insert(IEnumerable<PartnerQualification> partnerQualifications, CancellationToken cancellationToken);
    Task<IEnumerable<PartnerQualification>> GetAll(CancellationToken cancellationToken);
    Task<PartnerQualification?> GetByCode(int code, CancellationToken cancellationToken);
}
