using System;

[ParserCommand("v", "var", "variable")]
public class DisplayVariableCommand : DisplayTextCommand
{
    private string variableName;

    [ParserCommandConstructor]
    public DisplayVariableCommand(string variableName)
    {
        this.variableName = variableName;
    }

    private int cachedLength;
    protected override int Length { get { return cachedLength; } }

    public override string Evaluate(ICommandApplicationContext context)
    {
        var s = context.GetVariableValue<object>(variableName).ToString();
        cachedLength = s.Length;
        return s;
    }
}
