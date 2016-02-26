using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextWithEffects))]
public class TextWithEffectsEditor : UnityEditor.UI.TextEditor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Effects"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
