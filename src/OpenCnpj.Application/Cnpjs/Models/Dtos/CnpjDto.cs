namespace OpenCnpj.Application.Cnpjs.Models.Dtos;

public record CnpjDto(string BaseCnpj, string? OrderCnpj, string? VerifierDigits);
