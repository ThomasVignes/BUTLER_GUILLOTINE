using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildSave : MonoBehaviour
{
    [Header("Build save settings")]
    [SerializeField] string path;

    [Header("Editor save settings")]
    [SerializeField] string editorPath;

    [ContextMenu("PlaceholderSave")]
    public void PlaceholderSave()
    {
        var the = new BuildSaveData(true, true, true, true, true, false, false, false);

        SaveTo(the);
    }

    public void SaveTo(BuildSaveData buildSaveData)
    {
        var json = JsonUtility.ToJson(buildSaveData, true);

        var fullPath = Application.dataPath + path + "/Saves.txt";

#if UNITY_EDITOR
        fullPath = Application.dataPath + editorPath + "/Saves.txt";
#endif

        if (File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, json);

            Debug.Log("Updated " + fullPath);
        }
        else
        {
            using (StreamWriter sw = File.CreateText(fullPath))
            {
                sw.WriteLine(json);
            }

            Debug.Log("Created file at " + fullPath);
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}

public class BuildSaveData
{
    public bool GameFinished;
    public bool Ceremony1Reached;
    public bool Ceremony2Reached;
    public bool Ceremony3Reached;
    public bool LongestDay1Reached;
    public bool LongestDay2Reached;
    public bool LongestDay3Reached;
    public bool LongestDay4Reached;
    public bool LongestDay5Reached;

    public BuildSaveData(bool ceremony1Reached, bool ceremony2Reached, bool ceremony3Reached, bool longestDay1Reached, 
        bool longestDay2Reached, bool longestDay3Reached, bool longestDay4Reached, bool longestDay5Reached)
    {
        Ceremony1Reached = ceremony1Reached;
        Ceremony2Reached = ceremony2Reached;
        Ceremony3Reached = ceremony3Reached;
        LongestDay1Reached = longestDay1Reached;
        LongestDay2Reached = longestDay2Reached;
        LongestDay3Reached = longestDay3Reached;
        LongestDay4Reached = longestDay4Reached;
        LongestDay5Reached = longestDay5Reached;
    }
}
