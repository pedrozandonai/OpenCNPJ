namespace OpenCnpj.Application.CompanySituations.Models.Dtos;
public record CompanySituationDto(short ID, string Description)
{ 
    public static CompanySituationDto CreateByID(short ID)
    {
        string description = string.Empty;

        switch(ID)
        {
            case 1:
                description = "NULA";
                break;

            case 2:
                description = "ATIVA";
                break;

            case 3:
                description = "SUSPENSA";
                break;

            case 4:
                description = "INAPTA";
                break;

            case 8:
                description = "BAIXADA";
                break;
        }

        return new CompanySituationDto(ID, description);
    }
}

