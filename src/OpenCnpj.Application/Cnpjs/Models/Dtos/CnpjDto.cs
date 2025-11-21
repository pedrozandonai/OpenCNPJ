namespace OpenCnpj.Application.Cnpjs.Models.Dtos;

public record CnpjDto
{
    public string BaseCnpj { get; set; }
    public string? OrderCnpj { get; set; }
    public string? VerifierDigits { get; set; }
    public string? Full => GetFullCnpj();
    public string? Formatted => GetFormattedCnpj();

    public CnpjDto(string baseCnpj, string? orderCnpj, string? verifierDigits)
    {
        BaseCnpj = baseCnpj;
        OrderCnpj = orderCnpj;
        VerifierDigits = verifierDigits;
    }

    private string? GetFullCnpj()
    {
        if (string.IsNullOrEmpty(BaseCnpj) ||
            string.IsNullOrEmpty(OrderCnpj) ||
            string.IsNullOrEmpty(VerifierDigits))
            return null;

        return BaseCnpj + OrderCnpj + VerifierDigits;
    }

    private string? GetFormattedCnpj()
    {
        var raw = GetFullCnpj();
        if (string.IsNullOrEmpty(raw))
            return null;

        // Esperado: 14 dígitos
        if (raw.Length != 14)
            return raw; // retorna sem formatar caso venha quebrado

        return $"{raw[..2]}.{raw.Substring(2, 3)}.{raw.Substring(5, 3)}/{raw.Substring(8, 4)}-{raw.Substring(12, 2)}";
    }
}
