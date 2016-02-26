using UnityEngine;

using System;
using System.IO;
using System.Text;

using System.Collections.Generic;
using System.Linq;

public class TextScript : MonoBehaviour
{
    public struct CharacterProperties
    {
        public int PauseIndex;
        public float DisplayTiming;
        public Color? Color;
        public AudioClip Sound;
    }

    public float DefaultDelay = 10;

    [TextArea]
    [SerializeField]
    private string script;

    public AudioClip[] CharacterSounds;

    public void OnEnable()
    {
        ParseScript();
    }

    private enum Command
    {
        DisplayCharacter,
        SetDelay,
        SetSpeed,
        SetColor,
        SetAudio,
        PlaySound,
        VariableValue,
        WaitForInput
    }
    private struct ScriptCommand
    {
        public Command Command;
        public object[] Args;

        public ScriptCommand(Command command, params object[] args)
        {
            Command = command;
            Args = args;
        }
    }

    private string text;
    private string parsed = string.Empty;
    public string ParseScript()
    {
        if (parsed == script) return Text;

        StringReader reader = new StringReader(script);

        List<ScriptCommand> commands = new List<ScriptCommand>();
        
        while (reader.Peek() >= 0)
        {
            switch (reader.Peek())
            {
                case '@':
                    {
                        var command = ParseCommand(reader);
                        if (command != null) commands.Add(command.Value);
                        break;
                    }

                default:
                    {
                        commands.Add(new ScriptCommand(Command.DisplayCharacter, (char)reader.Read()));
                        break;
                    }
            }
        }

        ApplyCommands(commands);
        parsed = script;
        return Text;
    }

    private const char PlaySoundMarker = '\u2063';
    private CharacterProperties[] Properties;
    private void ApplyCommands(IEnumerable<ScriptCommand> commands)
    {   StringBuilder builder = new StringBuilder();
        List<CharacterProperties> properties = new List<CharacterProperties>();

        float delay = 0;
        float delayIncrement = DefaultDelay;
        int pauseIndex = 0;
        Color? currentColor = null;

        AudioClip defaultSound = (CharacterSounds.Length > 0 ? CharacterSounds[0] : null);

        AudioClip sound = defaultSound;
        AudioClip punctuationSound = null;
        AudioClip playSound = null;

        System.Action<char> DisplayCharacter = x =>
        {
            builder.Append(x);
            if (!char.IsWhiteSpace(x) && x != PlaySoundMarker) delay += delayIncrement;

            AudioClip actualSound = x == PlaySoundMarker ? playSound :
                                    char.IsWhiteSpace(x) ? null :
                                    char.IsPunctuation(x) ? punctuationSound : sound;
            properties.Add(new CharacterProperties() { DisplayTiming = delay, Color = currentColor, Sound = actualSound, PauseIndex = pauseIndex });
        };

        foreach (ScriptCommand command in commands)
        {
            switch (command.Command)
            {
                case Command.DisplayCharacter:
                    {
                        DisplayCharacter((char)command.Args[0]);
                        break;
                    }

                case Command.SetDelay:
                    {
                        delay += (float)command.Args[0];
                        break;
                    }

                case Command.SetColor:
                    {
                        currentColor = (command.Args.Length > 0 ? (Color)command.Args[0] : (Color?)null);
                        break;
                    }

                case Command.SetSpeed:
                    {
                        delayIncrement = (command.Args.Length > 0 ? (float)command.Args[0] : DefaultDelay);
                        break;
                    }

                case Command.SetAudio:
                    {
                        int arg;
                        switch (command.Args.Length)
                        {
                            case 2:
                                punctuationSound = (arg = (int)command.Args[1]) >= 0 && arg < CharacterSounds.Length ? CharacterSounds[arg] : null;
                                goto case 1;

                            case 1:
                                sound = (arg = (int)command.Args[0]) >= 0 && arg < CharacterSounds.Length ? CharacterSounds[arg] : null;
                                if (command.Args.Length == 1) punctuationSound = null;
                                break;

                            default:
                                sound = defaultSound;
                                punctuationSound = null;
                                break;
                        }
                        break;
                    }

                case Command.VariableValue:
                    {
                        string v = GetVariableValue((string)command.Args[0]);
                        foreach (var c in v)
                        {
                            DisplayCharacter(c);
                        }
                        break;
                    }

                case Command.PlaySound:
                    {
                        int arg;
                        playSound = (arg = (int)command.Args[0]) >= 0 && arg < CharacterSounds.Length ? CharacterSounds[arg] : null;
                        DisplayCharacter(PlaySoundMarker);
                        playSound = null;
                        break;
                    }

                case Command.WaitForInput:
                    {
                        pauseIndex++;
                        delay = 0;
                        break;
                    }
            }
        }

        text = builder.ToString();
        Properties = properties.ToArray();
    }

    private string GetVariableValue(string variableName)
    {
        return "XXXXXXX";
    }

    private ScriptCommand? ParseCommand(StringReader reader)
    {
        reader.Read();
        switch (reader.Peek())
        {
            case '@':
                reader.Read();
                return new ScriptCommand(Command.DisplayCharacter, '@');

            default:
                string command = "";
                int peek;
                while ((peek = reader.Peek()) != ';' && peek != '(' && peek != -1)
                {
                    command += (char)reader.Read();
                }
                if (peek == ';') reader.Read();
                if (peek != '(') return ParseCommand(command);
                reader.Read();

                string argString = "";
                while ((peek = reader.Read()) != ')' && peek != -1)
                {
                    argString += (char)peek;
                }
                return ParseCommand(command, argString.Split(' '));
        }
    }

    private float ParseFloatOrDefault(string num, float def = 0)
    {
        float res;
        if (float.TryParse(num, out res)) return res;
        return def;
    }

    private int ParseIntOrDefault(string num, int def = 0)
    {
        int res;
        if (int.TryParse(num, out res)) return res;
        return def;
    }

    private bool ParseBoolOrDefault(string val, bool def = false)
    {
        bool res;
        if (bool.TryParse(val, out res)) return res;
        return def;
    }

    private ScriptCommand? ParseCommand(string command, params string[] args)
    {
        switch (command.ToLower())
        {
            case "delay":
            case "d":
                return new ScriptCommand(Command.SetDelay, ParseFloatOrDefault(args[0]));

            case "color":
            case "c":
                if (args.Length == 0)
                {
                    return new ScriptCommand(Command.SetColor);
                }

                Color color = Color.white;
                
                if (args[0][0] == '#')
                {
                    string hex = args[0].Substring(1);
                    int val = System.Convert.ToInt32(hex, 16);

                    color.b = (val & 255) / 255f;
                    color.g = ((val >> 8) & 255) / 255f;
                    color.r = ((val >> 16) & 255) / 255f;
                }

                return new ScriptCommand(Command.SetColor, color);

            case "speed":
            case "s":
                if (args.Length == 0) return new ScriptCommand(Command.SetSpeed);
                float speed = ParseFloatOrDefault(args[0]);
                return new ScriptCommand(Command.SetSpeed, speed);

            case "audio":
            case "aud":
                var arr = new object[args.Length];
                switch (args.Length)
                {
                    case 3:
                        arr[2] = ParseIntOrDefault(args[2]);
                        goto case 2;

                    case 2:
                        arr[1] = ParseIntOrDefault(args[1]);
                        goto case 1;

                    case 1:
                        arr[0] = ParseIntOrDefault(args[0]);
                        goto default;

                    default:
                        return new ScriptCommand(Command.SetAudio, arr);
                }

            case "play":
            case "p":
                return new ScriptCommand(Command.PlaySound, ParseIntOrDefault(args[0]));

            case "var":
            case "v":
                return new ScriptCommand(Command.VariableValue, args[0]);

            case "wait":
            case "w":
                return new ScriptCommand(Command.WaitForInput);

            default:
                return null;
        }
    }

    private T[] SubsetRange<T>(T[] source, int start, int length)
    {
        if (length == 0) return new T[0];

        T[] arr = new T[length];
        Array.Copy(source, start, arr, 0, length);
        return arr;
    }

    public int GetNumCharactersToDisplay(int pauseIndex, float startTime)
    {
        for (int i = 0; i < Properties.Length; i++)
        {
            if (!ShouldDisplay(i, pauseIndex, startTime)) return i;
        }
        return Properties.Length;
    }

    public bool ShouldDisplay(int charIndex, int pauseIndex, float startTime)
    {
        if (!CharacterInRange(charIndex)) return true;
        var p = Properties[charIndex];
        return (p.PauseIndex == pauseIndex && p.DisplayTiming * 0.01f < Time.time - startTime)
            || p.PauseIndex < pauseIndex;
    }

    public bool IsPaused(int pauseIndex, float startTime)
    {
        float time = (Time.time - startTime) * 100;
        for (int i = 0; i < Properties.Length; i++)
        {
            var p = Properties[i];
            if (p.PauseIndex > pauseIndex)
            {
                switch (i)
                {
                    case 0:
                        return true;

                    default:
                        var p2 = Properties[i - 1];
                        return p2.PauseIndex > pauseIndex
                            || p2.PauseIndex == pauseIndex
                            && p2.DisplayTiming <= time;
                }
            }

        }
        return false;
    }

    public Color? ColorOverride(int charIndex)
    {
        return (CharacterInRange(charIndex) ? Properties[charIndex].Color : null);
    }

    public AudioClip CharacterSound(int startingIndex, int endingIndex, int pauseIndex)
    {
        for (int i = Mathf.Max(0, startingIndex); i < endingIndex; i++)
        {
            AudioClip s;
            if ((s = Properties[i].Sound) != null) return s;
        }
        return null;
    }

    private bool CharacterInRange(int index)
    {
        return Properties != null 
            && index < Properties.Length
            && index >= 0;
    }

    public string Text
    {
        get { return text; }
    }
}
