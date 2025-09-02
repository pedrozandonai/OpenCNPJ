using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.LegalNatures.Services;
public interface ILegalNatureService
{
    Task<Result> CreateLegalNatures(CancellationToken cancellationToken);
}