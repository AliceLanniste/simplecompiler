using System;

public sealed class VariableSymbol
{
    public VariableSymbol(String name, Type type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public Type Type { get; }
}