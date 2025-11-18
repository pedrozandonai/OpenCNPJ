using OpenCnpj.Application.RawRecords;

namespace OpenCnpj.Application.Establishments.Models;
public class EstablishmentDetailedInformation : EstablishmentRawRecord
{
    public CnaeRawRecord MainCnae { get; set; }
}
