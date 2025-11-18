namespace OpenCnpj.Application.Ages.Models.Dtos;
public record AgeDto(short ID, string Description)
{
    public static AgeDto CreateByID(short ID)
    {
        string description;

        switch (ID)
        {
            case 0:
                description = "NÃO SE APLICA";
                break;
            case 1:
                description = "ENTRE 0 A 12 ANOS";
                break;
            case 2:
                description = "ENTRE 13 A 20 ANOS";
                break;
            case 3:
                description = "ENTRE 21 A 30 ANOS";
                break;
            case 4:
                description = "ENTRE 31 A 40 ANOS";
                break;
            case 5:
                description = "ENTRE 41 A 50 ANOS";
                break;
            case 6:
                description = "ENTRE 51 A 60 ANOS";
                break;
            case 7:
                description = "ENTRE 61 A 70 ANOS";
                break;
            case 8:
                description = "ENTRE 71 A 80 ANOS";
                break;
            default:
                return new(default, string.Empty);
        }

        return new(ID, description);
    }
}

