using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.WebApi.Services.CsvProcessingServices.Strategy;
using OpenCnpj.WebApi.Services.CsvProcessingServices.Strategy.Strategies;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.WebApi.Services.CsvProcessingServices.Strategy.Factory;
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

        _strategies["Paises"] = new ReferenceDataProcessingStrategy<CountryRawRecord>("Paises", mongoDatabaseFactory, tweakSettings, logger);

        _strategies["Municipios"] = new ReferenceDataProcessingStrategy<CityRawRecord>("Municipios", mongoDatabaseFactory, tweakSettings, logger);

        _strategies["Qualificacoes"] = new ReferenceDataProcessingStrategy<PartnerQualificationRawRecord>("Qualificacoes", mongoDatabaseFactory, tweakSettings, logger);

        _strategies["Naturezas"] = new ReferenceDataProcessingStrategy<LegalNatureRawRecord>("Naturezas", mongoDatabaseFactory, tweakSettings, logger);

        _strategies["Cnaes"] = new ReferenceDataProcessingStrategy<CnaeRawRecord>("Cnaes", mongoDatabaseFactory, tweakSettings, logger);

        _strategies["Motivos"] = new ReferenceDataProcessingStrategy<ReasonRawRecord>("Motivos", mongoDatabaseFactory, tweakSettings, logger);
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
