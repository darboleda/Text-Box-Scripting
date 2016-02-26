using System;

[AttributeUsage(AttributeTargets.Constructor)]
public class ParserCommandConstructorAttribute : Attribute
{
    public ParserCommandConstructorAttribute() { }
}
