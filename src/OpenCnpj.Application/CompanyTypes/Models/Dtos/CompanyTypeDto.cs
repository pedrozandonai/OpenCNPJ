namespace OpenCnpj.Application.CompanyTypes.Models.Dtos;
public record CompanyTypeDto(short ID, string Description)
{
    public static CompanyTypeDto CreateByID(short ID)
    {
        string description;
        switch (ID)
        {
            case 1:
                description = "MATRIZ";
                break;
            case 2:
                description = "FILIAL";
                break;
            default:
                return new(default, string.Empty);
        }

        return new(ID, description);
    }
}
