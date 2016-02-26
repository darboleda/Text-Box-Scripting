using System.Collections;

using UnityEngine;

[ParserCommand("sfx")]
public class PlaySoundEffect : ScriptCommand
{
    private int soundIndex;
    private float? timeout;

    [ParserCommandConstructor]
    public PlaySoundEffect(int soundIndex)
    {
        this.soundIndex = soundIndex;
        this.timeout = null;
    }

    [ParserCommandConstructor]
    public PlaySoundEffect(int soundIndex, float timeout)
    {
        this.soundIndex = soundIndex;
        this.timeout = timeout;
    }

    public override IEnumerator Activate(ICommandActivationContext context)
    {
        context.PlaySoundClip(soundIndex);
        if (timeout == null) yield break;
        yield return new WaitForSeconds(timeout.Value * context.TimeUnit);
    }
}
