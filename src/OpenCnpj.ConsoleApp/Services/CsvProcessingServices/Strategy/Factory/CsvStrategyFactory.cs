using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Configurations;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Strategies;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Factory;
public class CsvStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, ICsvProcessingStrategy> _strategies;

    public CsvStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _strategies = new Dictionary<string, ICsvProcessingStrategy>(StringComparer.OrdinalIgnoreCase);

        RegisterStrategies();
    }

    private void RegisterStrategies()
    {
        // Estratégias específicas
        _strategies["Empresas"] = _serviceProvider.GetRequiredService<CompanyProcessingStrategy>();
        _strategies["Estabelecimento"] = _serviceProvider.GetRequiredService<EstablishmentProcessingStrategy>();
        _strategies["Socios"] = _serviceProvider.GetRequiredService<PartnerProcessingStrategy>();
        _strategies["Simples"] = _serviceProvider.GetRequiredService<SimpleDataProcessingStrategy>();

        // Estratégias de dados de referência
        var mongoDatabaseFactory = _serviceProvider.GetRequiredService<IMongoDatabaseFactory>();
        var batchSettings = _serviceProvider.GetRequiredService<BatchSettings>();
        var logger = _serviceProvider.GetRequiredService<ILogger>();

        _strategies["Paises"] = new ReferenceDataProcessingStrategy<CountryRawRecord>("Paises", mongoDatabaseFactory, batchSettings, logger);

        _strategies["Municipios"] = new ReferenceDataProcessingStrategy<CityRawRecord>("Municipios", mongoDatabaseFactory, batchSettings, logger);

        _strategies["Qualificacoes"] = new ReferenceDataProcessingStrategy<PartnerQualificationRawRecord>("Qualificacoes", mongoDatabaseFactory, batchSettings, logger);

        _strategies["Naturezas"] = new ReferenceDataProcessingStrategy<LegalNatureRawRecord>("Naturezas", mongoDatabaseFactory, batchSettings, logger);

        _strategies["Cnaes"] = new ReferenceDataProcessingStrategy<CnaeRawRecord>("Cnaes", mongoDatabaseFactory, batchSettings, logger);

        _strategies["Motivos"] = new ReferenceDataProcessingStrategy<ReasonRawRecord>("Motivos", mongoDatabaseFactory, batchSettings, logger);
    }

    public ICsvProcessingStrategy? GetStrategy(string fileName)
    {
        foreach (var (pattern, strategy) in _strategies)
        {
            if (fileName.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return strategy;
            }
        }

        return null;
    }

    public IEnumerable<string> GetSupportedFilePatterns()
    {
        return _strategies.Keys;
    }
}
