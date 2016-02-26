using System;
using System.Collections;

public class DisplayStringCommand : DisplayTextCommand
{
    private string s;
    public DisplayStringCommand(string s) { this.s = s; }

    protected override int Length { get { return s.Length; } }

    public override string Evaluate(ICommandApplicationContext context) { return s; }
}
