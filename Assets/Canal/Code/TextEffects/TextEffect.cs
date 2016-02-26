using UnityEngine;
using System.Collections;

public class TextEffect : MonoBehaviour
{
    public bool Apply(int characterIndex, UIVertex[] verts)
    {
        return !enabled || ApplyEffect(characterIndex, verts);
    }

    protected virtual bool ApplyEffect(int characterIndex, UIVertex[] verts) { return true; }
}
