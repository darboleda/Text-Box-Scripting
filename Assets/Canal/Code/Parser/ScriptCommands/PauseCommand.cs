using System.Collections;

using UnityEngine;

[ParserCommand("p", "pause")]
public class PauseCommand : ScriptCommand
{
    private float timeout;
    private bool isSkippable;

    [ParserCommandConstructor]
    public PauseCommand(float timeout)
    {
        this.timeout = timeout;
        isSkippable = true;
    }

    [ParserCommandConstructor]
    public PauseCommand(float timeout, bool isSkippable)
    {
        this.timeout = timeout;
        this.isSkippable = isSkippable;
    }

    public override IEnumerator Activate(ICommandActivationContext context)
    {
        float time = 0;
        float realTimeout = timeout * context.TimeUnit;
        while (time < realTimeout)
        {
            yield return null;
            time += Time.deltaTime * (isSkippable ? context.TimeScale : 1);
        }
    }

    public override bool Skip(ICommandActivationContext context)
    {
        return isSkippable;
    }
}
