namespace OpenCnpj.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PgTableAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
