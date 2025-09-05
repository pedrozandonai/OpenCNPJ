using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.PartnersQualifications.Services;
public interface IPartnerQualificationService
{
    Task<Result> CreatePartnerQualifications(CancellationToken cancellationToken);
}