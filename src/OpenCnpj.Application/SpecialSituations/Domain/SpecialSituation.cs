using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.SpecialSituations.Domain;

[PgTable("special_situations")]
public class SpecialSituation
{
    [PgColumn("id")]
    public long ID { get; private set; }

    [PgColumn("description")]
    public string Description { get; private set; }

    public SpecialSituation(long id, string description)
    {
        ID = id;
        Description = description;
    }

    public static SpecialSituation Create(long id, string description)
        => new (id, description);
}
