namespace OpenCnpj.Application.Companies.CompanySizes.Models.Dtos;
public record CompanySizeDto(short ID, string Description)
{ 
    public static CompanySizeDto CreateByID(short? ID)
    {
        string description;

        switch(ID)
        {
            case 0:
                description = "NÃO INFORMADO";
                break;
            case 1:
                description = "MICRO EMPRESA";
                break;
            case 3:
                description = "EMPRESA DE PEQUENO PORTE";
                break;
            case 5:
                description = "DEMAIS";
                break;
            default:
                return new(0, string.Empty);
        }

        return new(ID.Value, description);
    }
}

