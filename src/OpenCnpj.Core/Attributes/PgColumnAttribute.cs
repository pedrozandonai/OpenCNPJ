namespace OpenCnpj.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PgColumnAttribute(string name, int order = 0) : Attribute
{
    public string Name { get; } = name;
    public int Order { get; } = order;
}

