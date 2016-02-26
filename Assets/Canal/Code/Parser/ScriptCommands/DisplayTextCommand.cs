using System.Collections;

using UnityEngine;

public abstract class DisplayTextCommand : ScriptCommand
{
    protected abstract int Length { get; }


    private int stoppedIndex;
    public override IEnumerator Activate(ICommandActivationContext context)
    {
        float startTime = Time.time;
        stoppedIndex = 0;
        int currentIndex = context.LastDisplayedCharacterIndex + 1;
        int finalIndex = currentIndex + Length;

        float currentTime = startTime;
        while (currentIndex < finalIndex)
        {
            for (int i = currentIndex; i < finalIndex; i++)
            {
                if (currentTime - startTime < context[i].Delay) break;
                context.Advance();
                stoppedIndex++;
            }
            yield return null;
            currentTime += Time.deltaTime * context.TimeScale;
            currentIndex = context.LastDisplayedCharacterIndex + 1;
        }
        yield break;
    }

    public override bool Skip(ICommandActivationContext context)
    {
        int currentIndex = context.LastDisplayedCharacterIndex + 1;
        int finalIndex = currentIndex + Length - stoppedIndex;

        while (context.LastDisplayedCharacterIndex + 1 < finalIndex)
        {
            context.Advance();
        }

        return true;
    }
}
