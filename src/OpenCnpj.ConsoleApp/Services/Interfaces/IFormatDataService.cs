using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Services.Interfaces;
public interface IFormatDataService
{
    Task<Result> FormatData(CancellationToken cancellationToken);
}