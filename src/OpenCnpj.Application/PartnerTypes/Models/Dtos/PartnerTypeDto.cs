namespace OpenCnpj.Application.PartnerTypes.Models.Dtos;
public record PartnerTypeDto(short ID, string Description)
{
    public static PartnerTypeDto CreateByID(short ID)
    {
        string description;

        switch (ID)
        {
            case 1:
                description = "PESSOA JURÍDICA";
                break;
            case 2:
                description = "PESSOA FÍSICA";
                break;
            case 3:
                description = "ESTRANGEIRO";
                break;
            default:
                return new(default, string.Empty);
        }

        return new(ID, description);
    }
}

