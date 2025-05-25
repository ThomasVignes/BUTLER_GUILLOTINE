#if UNITY_EDITOR
using Codice.Client.Common;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Whumpus.Editor;
using static UnityEditor.Progress;


public class AllTextExporter : EditorWindow
{
    [MenuItem("ButlerEngine/AllTextExporter", false, 1)]
    public static void ShowWindow()
    {
        GetWindow(typeof(AllTextExporter));
    }

    //public string path = @"E:\Unity\ActiveProjects\BUTLERGUILLOTINE\AllText.txt";
    public string path;
    
    public string[] scenes;

    int sceneCount;

    int wordCount;


    public void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("AllTextExporter exports a JSON file containing all dialogue written within the butler engine.");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Currently discriminates WriteComment() command from GameManger, asshole");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Scene select
        sceneCount = Mathf.Max(0, EditorGUILayout.IntField("Scene number", sceneCount));

        if (scenes != null && scenes.Length != sceneCount)
        {
            scenes = new string[sceneCount];
        }

        if (scenes != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scene paths (only add what's after Assets/Scenes/, and no extensions)");

            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = EditorGUILayout.TextField(scenes[i]);
            }
        }

        //Path
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("File path");
        EditorGUILayout.BeginHorizontal();
        path = EditorGUILayout.TextField(path);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button("Export"))
        {
            Debug.Log("Start exporting...");

            ExportDialogue(path, scenes);
        }
    }

    void ExportDialogue(string path, string[] scenes)
    {
        //Init data variables
        wordCount = 0;
        List<SceneTextContainer> containers = new List<SceneTextContainer>();

        foreach (var scenePath in scenes)
        {
            List<TextExporterDialogue> textExporterDialogues = new List<TextExporterDialogue>();
            List<TextExporterCinematic> textExporterCinematics = new List<TextExporterCinematic>();
            List<TextExporterCinematic> textExporterLocalCinematics = new List<TextExporterCinematic>();
            List<TextExportObject> textExportInteractables = new List<TextExportObject>();


            SearchScene(scenePath, textExporterDialogues, textExporterCinematics, textExporterLocalCinematics, textExportInteractables);


            containers.Add(new SceneTextContainer(GetSceneNameFromPath(scenePath), textExporterDialogues, textExporterCinematics,
                textExporterLocalCinematics, textExportInteractables));
        }


        AllTextContainer allData = new AllTextContainer(wordCount, containers);

        //Export to JSON
        var json = JsonUtility.ToJson(allData, true);

        File.WriteAllText(path, json);

        Debug.Log("Done!");
    }

    void SearchScene(string ScenePath, List<TextExporterDialogue> textExporterDialogues, List<TextExporterCinematic> textExporterCinematics, List<TextExporterCinematic> textExporterLocalCinematics, List<TextExportObject> textExportInteractables)
    {
        //Open scene
        //string scenePath = SceneManager.GetSceneByName(ScenePath).path;
        Scene scene = EditorSceneManager.OpenScene("Assets/Scenes/" + ScenePath + ".unity", OpenSceneMode.Single);

        //Search for everything
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        CinematicManager cinematicManager = FindObjectOfType<CinematicManager>();
        LocalCinematic[] localCinematics = FindObjectsOfType<LocalCinematic>();
        CommentInteractable[] commentInteractables = FindObjectsOfType<CommentInteractable>();
        Door[] doors = FindObjectsOfType<Door>();
        EventArea[] eventAreas = FindObjectsOfType<EventArea>();

        //Test
        foreach (var item in eventAreas)
        {
            SearchForEventDelegate(item.OnTrigger);
        }

        //Get content from dialogueManager
        foreach (var dialogue in dialogueManager.Dialogues)
        {
            foreach (var line in dialogue.Lines)
            {
                List<string> answers = new List<string>();

                foreach (var item in line.Answers)
                {
                    answers.Add(item.Text);

                    wordCount += CountWords(item.Text);
                }

                textExporterDialogues.Add(new TextExporterDialogue(line.Text, answers.ToArray()));

                Debug.Log("added dialogue");

                wordCount += CountWords(line.Text);
            }
        }

        //Get content from cinematicManager
        foreach (var cinematic in cinematicManager.Cinematics)
        {
            string id = cinematic.Data.ID;
            List<string> lines = new List<string>();

            foreach (var line in cinematic.Data.lines)
            {
                lines.Add(line.Text);

                wordCount += CountWords(line.Text);
            }

            textExporterCinematics.Add(new TextExporterCinematic(id, lines.ToArray(), false));

            Debug.Log("added cinematic");
        }

        //Get content from localCinematics
        foreach (var localCinematic in localCinematics)
        {
            string id = localCinematic.ID;
            List<string> lines = new List<string>();

            foreach (var line in localCinematic.lines)
            {
                lines.Add(line.Text);

                wordCount += CountWords(line.Text);
            }

            textExporterLocalCinematics.Add(new TextExporterCinematic(id, lines.ToArray(), true));

            Debug.Log("added local cinematic");
        }

        //Get content from commentInteractables
        foreach (var interactable in commentInteractables)
        {
            textExportInteractables.Add(new TextExportObject(interactable.gameObject.name, interactable.Comment));

            Debug.Log("added interactable");

            wordCount += CountWords(interactable.Comment);
        }

        //Get content from doors
        foreach (var door in doors)
        {
            textExportInteractables.Add(new TextExportObject(door.gameObject.name, door.LockedMessage));

            Debug.Log("added door");

            wordCount += CountWords(door.LockedMessage);
        }
    }

    int CountWords(string text)
    {
        int wordCount = 1;
        char lastChar = '.';

        foreach (var c in text)
        {
            if (c == ' ' && lastChar != ' ')
                wordCount++;

            lastChar = c;
        }

        return wordCount;
    }

    void SearchForEventDelegate(UnityEvent unityEvent)
    {
        int index = unityEvent.GetPersistentEventCount();

        for (int i = 0; i < index; i++)
        {
            if (unityEvent.GetPersistentMethodName(i) == "WriteComment")
            {
                //Find arguments from here
            }

            //Debug.Log(unityEvent.GetPersistentMethodName(i));
        }
    }

    string GetSceneNameFromPath(string path)
    {
        string SceneName = "";

        foreach (var c in path)
        {
            SceneName += c;

            if (c == '/')
                SceneName = "";
        }

        return SceneName;
    }
}


[Serializable]
public class AllTextContainer
{
    public int globalWordCount;
    public SceneTextContainer[] scenes;

    public AllTextContainer(int global, List<SceneTextContainer> containers)
    {
        globalWordCount = global;
        scenes = containers.ToArray();
    }
}

[Serializable]
public class SceneTextContainer
{
    public string sceneName;
    public TextExporterDialogue[] textExporterDialogues;
    public TextExporterCinematic[] textExporterCinematics;
    public TextExporterCinematic[] textExporterLocalCinematics;
    public TextExportObject[] textExportInteractables;

    public SceneTextContainer(string scName, List<TextExporterDialogue> d, List<TextExporterCinematic> c, List<TextExporterCinematic> lc, List<TextExportObject> i)
    {
        sceneName = scName;
        textExporterDialogues = d.ToArray();
        textExporterCinematics = c.ToArray();
        textExporterLocalCinematics = lc.ToArray();
        textExportInteractables = i.ToArray();
    }
}

[Serializable]
public class TextExporterDialogue
{
    public string text;
    public string[] answers;

    public TextExporterDialogue(string t, string[] a)
    {
        text = t;
        answers = a;
    }
}

[Serializable]
public class TextExporterCinematic
{
    public string ID;
    public string[] lines;
    public bool local;

    public TextExporterCinematic(string id, string[] l, bool lo)
    {
        ID = id;
        lines = l;
        local = lo;
    }
}

[Serializable]
public class TextExportObject
{
    public string ObjectName;
    public string text;

    public TextExportObject(string o, string t)
    {
        ObjectName = o;
        text = t;
    }
}
#endif
