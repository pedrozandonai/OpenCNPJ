using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.PartnersQualifications.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class PartnerQualificationRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), IPartnerQualificationRepository
{
    public async Task Insert(IEnumerable<PartnerQualification> partnerQualifications, CancellationToken cancellationToken)
        => await openCnpjDbContext.PartnerQualifications.AddRangeAsync(partnerQualifications, cancellationToken);

    public async Task<IEnumerable<PartnerQualification>> GetAll(CancellationToken cancellationToken)
        => await openCnpjDbContext.PartnerQualifications.ToListAsync(cancellationToken);

    public async Task<PartnerQualification?> GetByCode(int code, CancellationToken cancellationToken)
        => await openCnpjDbContext.PartnerQualifications
        .Where(l => l.Code == code)
        .FirstOrDefaultAsync(cancellationToken);
}
