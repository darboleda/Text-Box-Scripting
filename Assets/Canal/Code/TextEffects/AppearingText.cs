using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AppearingText : TextEffect
{
    public Text Text;
    public ScriptController Script;

    private float startTime;

    protected override bool ApplyEffect(int characterIndex, UIVertex[] verts)
    {
        if (!Application.isPlaying) return true;

        var props = Script[characterIndex];
        return props != null && props.Value.IsVisible;
    }
}
