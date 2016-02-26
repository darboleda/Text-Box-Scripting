using System.Collections;

[ParserCommand("a", "audio")]
public class SetSoundCommand : ScriptCommand
{
    private int index;

    [ParserCommandConstructor]
    public SetSoundCommand()
    {
        index = -1;
    }


    [ParserCommandConstructor]
    public SetSoundCommand(int index)
    {
        this.index = index;
    }

    public override void Apply(ICommandApplicationContext context)
    {
        context.CharacterSoundClipIndex = index;
    }
}
