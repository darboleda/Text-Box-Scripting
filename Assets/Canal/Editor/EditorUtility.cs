using UnityEngine;
using UnityEditor;

using System.IO;

public static class EditorHelper
{

    public static void CreateAsset<T>(string defaultName) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == string.Empty)
        {
            path = "Assets";
        }
        else if (Path.HasExtension(path))
        {
            path = Directory.GetParent(path).FullName;
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/{1}.asset", path, defaultName));

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Text/Create Script")]
    public static void TestCreate()
    {
        CreateAsset<FormattedScript>("New Formatted Script");
    }
}
