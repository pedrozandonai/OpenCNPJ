using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.LegalNatures.Services;
public interface ILegalNatureService
{
    Task<Result> CreateLegalNatures(CancellationToken cancellationToken);
}