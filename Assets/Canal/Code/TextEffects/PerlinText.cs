using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PerlinText : TextEffect
{
    public Vector2 TranslationSpeed = Vector2.one;
    public Vector2 TranslationMultiplier = Vector2.one;
    public Vector2 TranslationCharacterOffset = Vector2.one;
    public AnimationCurve TranslationXCurve;
    public AnimationCurve TranslationYCurve;
    private Vector2 translationPerlinNoise;

    public Text Text;

    public void Awake()
    {
        translationPerlinNoise = new Vector2(UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000));
        //Debug.LogFormat("{0} {1}", Mathf.PerlinNoise(translationPerlinNoise.x, TranslationCharacterOffset.x), Mathf.PerlinNoise(translationPerlinNoise.x, 0));
    }

    public void Update()
    {
        translationPerlinNoise += TranslationSpeed * Time.deltaTime;
        Text.SetVerticesDirty();
    }

    protected override bool ApplyEffect(int characterIndex, UIVertex[] verts)
    {
        float xOffset = TranslationXCurve.Evaluate(Mathf.PerlinNoise(translationPerlinNoise.x, characterIndex * TranslationCharacterOffset.x)) * TranslationMultiplier.x;
        float yOffset = TranslationYCurve.Evaluate(Mathf.PerlinNoise(characterIndex * TranslationCharacterOffset.y, translationPerlinNoise.y)) * TranslationMultiplier.y;

        for (int i = 0; i < verts.Length; i++)
        {
            verts[i].position.x += xOffset;
            verts[i].position.y += yOffset;
        }

        /*
        // LOL Let's do scaling instead
        Vector3 center = Vector3.zero;
        for (int i = 0; i < verts.Length; i++)
        {
            center += verts[i].position;
        }
        center *= 0.25f;
        for (int i = 0; i < verts.Length; i++)
        {
            var p = verts[i].position;
            p.x = (p.x - center.x) * (1 + xOffset) + center.x;
            p.y = (p.y - center.y) * (1 + yOffset) + center.y;
            verts[i].position = p;
        }
        */
        return true;
    }
}
