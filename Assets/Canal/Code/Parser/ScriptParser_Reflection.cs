using System;
using System.Reflection;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public partial class ScriptParser
{
    private static ScriptParser parser;
    public static ScriptParser Parser
    {
        get
        {
            parser = parser ?? new ScriptParser();
            return parser;
        }
    }

    private class CommandInfo
    {
        public Type Type;

        public Type[][] ArgsSettings;
        public ConstructorInfo[] Constructors;
    }

    private Dictionary<string, CommandInfo> commandMap = new Dictionary<string, CommandInfo>();

    private ScriptParser()
    {
        var commands = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttributes(typeof(ParserCommandAttribute), false).Any());
        foreach (var command in commands)
        {
            var cparser = new CommandInfo();
            cparser.Type = command;
            foreach (var c in ((ParserCommandAttribute)command.GetCustomAttributes(typeof(ParserCommandAttribute), false).First()).Commands)
            {
                commandMap[c] = cparser;
            }

            var constructors = new List<ConstructorInfo>(command.GetConstructors().Where(x => x.GetCustomAttributes(typeof(ParserCommandConstructorAttribute), true).Any()));
            var argSettings = new Type[constructors.Count][];

            for (int i = 0; i < constructors.Count; i++)
            {
                var constructor = constructors[i];
                argSettings[i] = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
            }

            cparser.ArgsSettings = argSettings;
            cparser.Constructors = constructors.ToArray();
        }
    }
}
