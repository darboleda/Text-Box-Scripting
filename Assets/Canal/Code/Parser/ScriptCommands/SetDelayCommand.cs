[ParserCommand("s", "speed")]
public class SetDelayCommand : ScriptCommand
{
    private float? delay;

    [ParserCommandConstructor]
    public SetDelayCommand()
    {
        this.delay = null;
    }

    [ParserCommandConstructor]
    public SetDelayCommand(float delay)
    {
        this.delay = delay;
    }

    public override void Apply(ICommandApplicationContext context)
    {
        context.DelayIncrement = delay;
    }
}
