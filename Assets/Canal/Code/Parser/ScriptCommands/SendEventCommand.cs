using System.Collections;

using UnityEngine;


[ParserCommand("e", "event")]
public class SendEventCommand : ScriptCommand
{
    private string eventName;
    private float delay;
    private bool skippable;

    [ParserCommandConstructor]
    public SendEventCommand(string eventName) : this(eventName, false) { }
    [ParserCommandConstructor]
    public SendEventCommand(string eventName, bool skippable) : this(eventName, 0, skippable) { }
    [ParserCommandConstructor]
    public SendEventCommand(string eventName, float delay) : this(eventName, delay, false) { }
    [ParserCommandConstructor]
    public SendEventCommand(string eventName, float delay, bool skippable)
    {
        this.eventName = eventName;
        this.delay = delay;
        this.skippable = skippable;
    }
    
    public override IEnumerator Activate(ICommandActivationContext context)
    {
        float time = 0;
        float realDelay = delay * context.TimeUnit;
        while (time < realDelay)
        {
            yield return null;
            time += Time.deltaTime * context.TimeScale;
        }
        context.SendEvent(eventName);
        yield break;
    }

    public override bool Skip(ICommandActivationContext context)
    {
        if (skippable)
        {
            return true;
        }
        return false;
    }
}
