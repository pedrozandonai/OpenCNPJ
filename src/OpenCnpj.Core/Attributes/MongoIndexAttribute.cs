namespace OpenCnpj.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MongoIndexAttribute : Attribute
{
    public bool Unique { get; }

    public MongoIndexAttribute(bool unique = false)
    {
        Unique = unique;
    }
}
