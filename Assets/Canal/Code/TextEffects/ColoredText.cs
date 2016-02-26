using UnityEngine;
using System.Collections;

public class ColoredText : TextEffect
{
    public ScriptController Script;

    protected override bool ApplyEffect(int characterIndex, UIVertex[] verts)
    {
        var p = Script[characterIndex];
        if (p != null && p.Value.Color != null)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i].color = p.Value.Color.Value;
            }
        }
        return true;
    }
}
