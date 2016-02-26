using System.Collections;

using UnityEngine;


[ParserCommand("w", "wait")]
public class WaitForEventCommand : ScriptCommand
{
    private string eventName;
    private float timeout;

    [ParserCommandConstructor]
    public WaitForEventCommand() : this(float.PositiveInfinity) { }

    [ParserCommandConstructor]
    public WaitForEventCommand(float timeout) : this("ContinueAction", timeout) { }

    [ParserCommandConstructor]
    public WaitForEventCommand(string eventName) : this(eventName, float.PositiveInfinity) { }

    [ParserCommandConstructor]
    public WaitForEventCommand(string eventName, float timeout)
    {
        this.eventName = eventName;
        this.timeout = timeout;
    }
    
    public override IEnumerator Activate(ICommandActivationContext context)
    {
        float time = 0;
        float realTimeout = timeout * context.TimeUnit;
        while (time < realTimeout)
        {
            yield return null;
            if (context.CheckEvent(eventName))
            {
                break;
            }
            time += Time.deltaTime * context.TimeScale;
        }
        yield break;
    }

    public override bool Skip(ICommandActivationContext context) { return false; }
}
