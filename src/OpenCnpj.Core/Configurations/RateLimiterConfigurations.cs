namespace OpenCnpj.Core.Configurations;

public class RateLimiterConfigurations
{
    public int PermitLimit { get; set; }
    public int WindowInSeconds { get; set; }
}