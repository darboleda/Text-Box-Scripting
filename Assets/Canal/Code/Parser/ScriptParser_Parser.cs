using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

public partial class ScriptParser
{
    public List<ScriptCommand> Parse(string input)
    {
        StringReader reader = new StringReader(input);
        var commands = new List<ScriptCommand>();

        string chain = "";
        while (reader.Peek() >= 0)
        {
            switch (reader.Peek())
            {
                case '@':
                    var command = ParseCommand(reader);
                    if (command != null)
                    {
                        if (chain != "")
                        {
                            if (chain[chain.Length - 1] == ' ')
                            {
                                while (reader.Peek() == ' ')
                                {
                                    reader.Read();
                                }
                            }
                            commands.Add(new DisplayStringCommand(chain));
                            chain = "";
                        }
                        commands.Add(command);
                    }
                    break;

                case '\n':
                    if(chain != "")
                    {
                        commands.Add(new DisplayStringCommand(chain));
                        chain = "";
                    }

                    reader.Read();
                    if (reader.Peek() == '\n')
                    {
                        while (char.IsWhiteSpace((char)reader.Peek()))
                        {
                            reader.Read();
                        }
                        commands.Add(new DisplayNewLineCommand());
                    }
                    break;

                case ' ':
                    reader.Read();
                    while (reader.Peek() == ' ')
                    {
                        reader.Read();
                    }
                    chain += " ";
                    break;

                default:
                    chain += (char)reader.Read();
                    break;
            }
        }
        if (chain != "")
        {
            commands.Add(new DisplayStringCommand(chain));
        }
        return commands;
    }

    private ScriptCommand ParseCommand(StringReader reader)
    {
        reader.Read();
        switch (reader.Peek())
        {
            case '@':
                reader.Read();
                return new DisplayStringCommand("@");

            default:
                string command = "";
                int peek;
                while ((peek = reader.Peek()) != ';' && peek != '(' && peek != -1)
                {
                    command += (char)reader.Read();
                }
                reader.Read();
                if (peek == ';' || peek != '(')
                {
                    return ParseCommand(command);
                }

                string argString = "";
                while ((peek = reader.Read()) != ')' && peek != -1)
                {
                    argString += (char)peek;
                }
                return ParseCommand(command, argString.Split(' '));
        }
    }

    private ScriptCommand ParseCommand(string command, params string[] args)
    {
        CommandInfo commandInfo;
        if (!commandMap.TryGetValue(command, out commandInfo)) return null;
        
        for (int i = 0; i < commandInfo.ArgsSettings.Length; i++)
        {
            var argSetting = commandInfo.ArgsSettings[i];
            if (argSetting.Length != args.Length) continue;

            int j;
            var parsedArgs = new object[args.Length];
            for (j = 0; j < args.Length; j++)
            {
                object val;
                if (!ParseToType(args[j], argSetting[j], out val)) break;
                parsedArgs[j] = val;
            }

            if (j != args.Length) continue;

            return commandInfo.Constructors[i].Invoke(parsedArgs) as ScriptCommand;
        }
        return null;
    }

    private bool ParseToType(string value, Type type, out object result)
    {
        result = value;
        if (type == typeof(string)) { return true; }
        if (type == typeof(int))
        {
            int res;
            if (!int.TryParse(value, out res)) return false;
            result = res;
            return true;
        }
        if (type == typeof(float))
        {
            float res;
            if (!float.TryParse(value, out res)) return false;
            result = res;
            return true;
        }
        if (type == typeof(bool))
        {
            bool res;
            if (!bool.TryParse(value, out res)) return false;
            result = res;
            return true;
        }
        if (type == typeof(Color))
        {
            Color color = Color.black;
            if (value[0] == '#')
            {
                string hex = value.Substring(1);
                int val;
                switch (hex.Length)
                {
                    case 6:
                        val = System.Convert.ToInt32(hex, 16);
                        color.b = (val & 255) / 255f;
                        color.g = ((val >> 8) & 255) / 255f;
                        color.r = ((val >> 16) & 255) / 255f;
                        break;

                    case 8:
                        val = System.Convert.ToInt32(hex, 16);
                        color.a = (val & 255) / 255f;
                        color.b = ((val >> 8) & 255) / 255f;
                        color.g = ((val >> 16) & 255) / 255f;
                        color.r = ((val >> 24) & 255) / 255f;
                        break;

                    case 3:
                        val = System.Convert.ToInt32(hex, 16);
                        color.b = (val & 15) / 15f;
                        color.g = ((val >> 4) & 15) / 15f;
                        color.r = ((val >> 8) & 15) / 15f;
                        break;

                    case 4:
                        val = System.Convert.ToInt32(hex, 16);
                        color.a = (val & 15) / 15f;
                        color.b = ((val >> 4) & 15) / 15f;
                        color.g = ((val >> 8) & 15) / 15f;
                        color.r = ((val >> 12) & 15) / 15f;
                        break;

                    default:
                        break;
                }
            }
            result = color;
            return true;
        }
        if (type.IsEnum)
        {
            result = Enum.Parse(type, value);
            return true;
        }

        return false;
    }
}

public interface ICommandApplicationContext
{
    T GetVariableValue<T>(string variableName);
    Color? Color { get; set; }
    int CharacterSoundClipIndex { get; set; }
    float? DelayIncrement { get; set; }
}

public interface ICharacterInfo
{
    float Delay { get; }
}

public interface ICommandActivationContext
{
    bool CheckEvent(string eventName);
    void SendEvent(string eventName);

    int LastDisplayedCharacterIndex { get; }

    float TimeUnit { get; }
    float TimeScale { get; }

    ICharacterInfo this[int index] { get; }
    int Advance();

    AudioClip PlaySoundClip(int index);
}
