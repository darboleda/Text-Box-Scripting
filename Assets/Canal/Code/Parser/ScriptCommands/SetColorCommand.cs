using UnityEngine;

[ParserCommand("c", "color")]
public class SetColorCommand : ScriptCommand
{
    private Color? color;

    [ParserCommandConstructor]
    public SetColorCommand()
    {
        color = null;
    }

    [ParserCommandConstructor]
    public SetColorCommand(Color color)
    {
        this.color = color;
    }

    public override void Apply(ICommandApplicationContext context)
    {
        context.Color = this.color;
    }
}
