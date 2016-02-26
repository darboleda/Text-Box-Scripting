using System;
using System.Collections;

[ParserCommand("n")]
public class DisplayNewLineCommand : DisplayTextCommand
{
    [ParserCommandConstructor]
    public DisplayNewLineCommand() { }
    protected override int Length { get { return 1; } }
    public override string Evaluate(ICommandApplicationContext context) { return "\n"; }
}
