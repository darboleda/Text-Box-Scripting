using UnityEngine;
using System.Collections.Generic;

public class FormattedScript : ScriptableObject
{
    public float DefaultDelay;

    [SerializeField]
    [TextArea]
    private string script;

    public float TimeUnit = 0.01f;
    public float SpeedUpTimeScale = 10;

    public AudioClip[] Sounds;

    private string parsed;
    private List<ScriptCommand> cachedCommands;
    public bool Parse(out List<ScriptCommand> commands)
    {
        if (cachedCommands != null && parsed == script)
        {
            commands = cachedCommands;
            return false;
        }

        ScriptParser parser = ScriptParser.Parser;
        commands = cachedCommands = parser.Parse(script);        
        parsed = script;
        return true;
    }
}
