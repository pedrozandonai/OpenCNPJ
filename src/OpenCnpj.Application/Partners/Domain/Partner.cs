using OpenCnpj.Application.Companies.Domain;
using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Application.Enums;
using OpenCnpj.Application.LegalRepresentatives.Domain;
using OpenCnpj.Application.PartnersQualifications.Domain;

namespace OpenCnpj.Application.Partners.Domain;
public class Partner
{
    public long Id { get; private set; }
    public Company Company { get; private set; }
    public EPartnerType PartnerType { get; private set; }
    public LegalRepresentative? LegalRepresentative { get; private set; }
    public PartnerQualification PartnerQualification { get; private set; }
    public string Name { get; private set; }
    public string? Identifier { get; private set; }
    public DateOnly StartDate { get; private set; }
    public Country Country { get; private set; }
    public EAgeRanges AgeRange { get; private set; }

    private Partner(long id, Company company, EPartnerType partnerType, LegalRepresentative? legalRepresentative, PartnerQualification partnerQualification, string name, string? identifier, DateOnly startDate, Country country, EAgeRanges ageRange)
    {
        Id = id;
        Company = company;
        PartnerType = partnerType;
        LegalRepresentative = legalRepresentative;
        PartnerQualification = partnerQualification;
        Name = name;
        Identifier = identifier;
        StartDate = startDate;
        Country = country;
        AgeRange = ageRange;
    }

    private Partner()
    {
    }

    public static Partner Create(Company company, EPartnerType partnerType, LegalRepresentative? legalRepresentative, PartnerQualification partnerQualification, string name, string? identifier, DateOnly startDate, Country country, EAgeRanges ageRange)
        => new(0, company, partnerType, legalRepresentative, partnerQualification, name, identifier, startDate, country, ageRange);
}
