#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;

public class SceneInstantiation : ISceneTemplatePipeline {
    
    public bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset) {
        return true;
    }

    public void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName) {
        
    }

    public void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName) {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if(!settings) {
            Debug.Log("No Addressable Settings Found!");
            return;
        }
        
        string path = "";
        while(path.Length == 0) {
            path = EditorUtility.SaveFilePanel(
                "Save Scene to...",
                Path.Combine(Application.dataPath, "Scenes"),
                "",
                "unity");
            if(path.Length != 0 && !IsSubPathOf(path, Application.dataPath)) {
                if(!EditorUtility.DisplayDialog("Invalid Path",
                    "Path needs to be inside the Assets folder or one of its subfolders", "Ok", "Cancel")) {
                    return;
                }

                path = "";
            } else if (path.Length == 0) break;
        }

        if(path.Length == 0) return;

        path = MakeRelative(path, Application.dataPath);
        string name = Path.GetFileNameWithoutExtension(path);

        EditorSceneManager.SaveScene(scene, path);
        
        var group = settings.FindGroup(name);
        if (!group)
            group = settings.CreateGroup(name, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
        
        var e = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
        var entriesAdded = new List<AddressableAssetEntry> {e};

        group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
    }
    
    public static string MakeRelative(string filePath, string referencePath)
    {
        var fileUri = new Uri(filePath);
        var referenceUri = new Uri(referencePath);
        return Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString()).Replace('/', Path.DirectorySeparatorChar);
    }
    
    /// <summary>
    /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
    /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
    /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
    /// </summary>
    public static bool IsSubPathOf(string path, string baseDirPath)
    {
        string normalizedPath = Path.GetFullPath(WithEnding(path.Replace('/', '\\'), "\\"));

        string normalizedBaseDirPath = Path.GetFullPath(WithEnding(baseDirPath.Replace('/', '\\'), "\\"));

        return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
    /// results in satisfying .EndsWith(ending).
    /// </summary>
    /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
    public static string WithEnding(string str, string ending)
    {
        if (str == null)
            return ending;

        string result = str;

        // Right() is 1-indexed, so include these cases
        // * Append no characters
        // * Append up to N characters, where N is ending length
        for (int i = 0; i <= ending.Length; i++)
        {
            string tmp = result + Right(ending, i);
            if (tmp.EndsWith(ending))
                return tmp;
        }

        return result;
    }

    /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
    /// <param name="value">The string to retrieve the substring from.</param>
    /// <param name="length">The number of characters to retrieve.</param>
    /// <returns>The substring.</returns>
    public static string Right(string value, int length)
    {
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
        }

        return (length < value.Length) ? value.Substring(value.Length - length) : value;
    }
}
#endif