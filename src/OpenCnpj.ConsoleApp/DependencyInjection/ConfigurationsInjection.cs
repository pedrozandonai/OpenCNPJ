using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.Core.Configurations;

namespace OpenCnpj.ConsoleApp.DependencyInjection;
public static class ConfigurationsInjection
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        GovSetttings govSetttings = new();

        configuration.GetRequiredSection(nameof(GovSetttings)).Bind(govSetttings);

        var govSettingsBuildReusult = govSetttings.Verify();
        if (govSettingsBuildReusult.IsFailure)
            throw new Exception(govSettingsBuildReusult.Error);

        services.AddSingleton(govSetttings);

        TweakSettings tweakSettings = new();

        configuration.GetRequiredSection(nameof(TweakSettings)).Bind(tweakSettings);

        services.AddSingleton(tweakSettings);

        DatabaseSettings databaseSettings = new();

        configuration.GetRequiredSection(nameof(DatabaseSettings)).Bind(databaseSettings);

        services.AddSingleton(databaseSettings);

        return services;
    }
}
