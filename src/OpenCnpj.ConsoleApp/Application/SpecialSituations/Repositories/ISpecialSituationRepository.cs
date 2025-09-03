using OpenCnpj.ConsoleApp.Application.SpecialSituations.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.SpecialSituations.Repositories;

public interface ISpecialSituationRepository : IOpenCnpjDatabaseFactory
{
    Task<long> Insert(SpecialSituation specialSituation, CancellationToken cancellationToken);
}