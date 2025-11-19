using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Strategies;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Factory;
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
        var tweakSettings = _serviceProvider.GetRequiredService<TweakSettings>();
        var logger = _serviceProvider.GetRequiredService<ILogger>();

        _strategies["Paises"] = new ReferenceDataProcessingStrategy<CountriesCollection>("Paises", mongoDatabaseFactory, tweakSettings, logger);
        _strategies["Municipios"] = new ReferenceDataProcessingStrategy<CitiesCollection>("Municipios", mongoDatabaseFactory, tweakSettings, logger);
        _strategies["Qualificacoes"] = new ReferenceDataProcessingStrategy<QualificationsCollection>("Qualificacoes", mongoDatabaseFactory, tweakSettings, logger);
        _strategies["Naturezas"] = new ReferenceDataProcessingStrategy<LegalNaturesCollection>("Naturezas", mongoDatabaseFactory, tweakSettings, logger);
        _strategies["Cnaes"] = new ReferenceDataProcessingStrategy<EconomicActivitiesCollection>("Cnaes", mongoDatabaseFactory, tweakSettings, logger);
        _strategies["Motivos"] = new ReferenceDataProcessingStrategy<ReasonsCollection>("Motivos", mongoDatabaseFactory, tweakSettings, logger);
    }

    public ICsvProcessingStrategy? GetStrategy(string fileName)
    {
        foreach (var (pattern, strategy) in _strategies)
            if (fileName.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
                return strategy;

        return null;
    }

    public IEnumerable<string> GetSupportedFilePatterns()
    {
        return _strategies.Keys;
    }
}
