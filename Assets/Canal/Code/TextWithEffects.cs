﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class TextWithEffects : Text
{
    public TextEffect[] Effects;

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (font == null)
            return;

        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.Populate(text, settings);

        Rect inputRect = rectTransform.rect;

        // get the text alignment anchor point for the text in local space
        Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
        Vector2 refPoint = Vector2.zero;
        refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
        refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);

        // Determine fraction of pixel to offset text mesh.
        Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        //Last 4 verts are always a new line...
        int vertCount = verts.Count - 4;

        toFill.Clear();
        ModifyVertices(verts, vertCount, toFill, roundingOffset, unitsPerPixel);
        m_DisableFontTextureRebuiltCallback = false;
    }

    readonly UIVertex[] m_TempVerts = new UIVertex[4];
    protected void ModifyVertices(IList<UIVertex> verts, int vertCount, VertexHelper toFill, Vector2 roundingOffset, float unitsPerPixel)
    {
        bool shouldRound = roundingOffset != Vector2.zero;

        for (int i = 0; i < vertCount; ++i)
        {
            int tempVertsIndex = i & 3;
            m_TempVerts[tempVertsIndex] = verts[i];
            m_TempVerts[tempVertsIndex].position *= unitsPerPixel;

            if (shouldRound)
            {
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
            }

            if (tempVertsIndex == 3)
            {
                int charIndex = i >> 2;
                bool shouldDraw = true;
                foreach (var effect in Effects)
                {
                    shouldDraw = shouldDraw && effect.Apply(charIndex, m_TempVerts);
                }

                if (shouldDraw)
                {
                    toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
        }
    }
}
