namespace OpenCnpj.Application.Simples.Models.Dtos;

public record class SimplesDto(bool? OptInSimple, DateTime? SimpleOptionDate, DateTime? SimpleExclusionDate, bool? OptInMei, DateTime? MeiOptionDate, DateTime? MeiExclusionDate);
