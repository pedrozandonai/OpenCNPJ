using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.PartnersQualifications.Services;
public interface IPartnerQualificationService
{
    Task<Result> CreatePartnerQualifications(CancellationToken cancellationToken);
}