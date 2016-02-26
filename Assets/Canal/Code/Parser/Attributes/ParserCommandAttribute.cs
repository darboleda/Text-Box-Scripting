using System;
using System.Collections.Generic;
using System.Linq;

[AttributeUsage(AttributeTargets.Class)]
public class ParserCommandAttribute : Attribute
{
    private string[] commands;
    public ParserCommandAttribute(params string[] commands)
    {
        this.commands = commands;
    }

    public IEnumerable<string> Commands { get { return commands.AsEnumerable(); } }
}
