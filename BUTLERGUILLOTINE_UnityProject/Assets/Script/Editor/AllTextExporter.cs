#if UNITY_EDITOR
using Codice.Client.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
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

    public string menuScene;

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

        //Menu Scene select
        EditorGUILayout.LabelField("Menu scene name");
        menuScene = EditorGUILayout.TextField(menuScene);

        //Scene select
        sceneCount = Mathf.Max(0, EditorGUILayout.IntField("Scene number", sceneCount));

        if (scenes == null)
            scenes = new string[sceneCount];

        if (scenes.Length != sceneCount)
        {
            scenes = new string[sceneCount];
        }

        if (scenes != null)
        {
            EditorGUILayout.Space();

            if (sceneCount > 0)
                EditorGUILayout.LabelField("Scene names");

            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = EditorGUILayout.TextField(scenes[i]);
            }
        }

        /*
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Path to items directory");
        itemsDirectory = EditorGUILayout.TextField(itemsDirectory);
        */

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
        List<TextExportItem> textExportItems = new List<TextExportItem>();
        List<TextExportDialogueBatch> textExportDialogueBatches = new List<TextExportDialogueBatch>();

        //Init scene container variables
        List<MenuSceneTextContainer> menuContainers = new List<MenuSceneTextContainer>();
        List<SceneTextContainer> containers = new List<SceneTextContainer>();

        //Search for scenes
        string menuScenePath = "EMPTY";
        List<string> scenePathsToSearch = new List<string>();
        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });

        foreach (var guid in sceneGUIDs)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            if (scenes.Contains(GetSceneNameFromPath(scenePath)))
                scenePathsToSearch.Add(scenePath);
            else if (GetSceneNameFromPath(scenePath) == menuScene)
                menuScenePath = scenePath;
        }

        //Search menu scene
        if (menuScenePath != "EMPTY")
        {
            List<TextExporterTextMeshPro> textExporterTextMeshPros = new List<TextExporterTextMeshPro>();
            SearchSceneAsMenu(menuScenePath, textExporterTextMeshPros);
            menuContainers.Add(new MenuSceneTextContainer(GetSceneNameFromPath(menuScenePath), textExporterTextMeshPros));
        }

        //Search scenes
        foreach (var scenePath in scenePathsToSearch)
        {
            List<TextExporterDialogue> textExporterDialogues = new List<TextExporterDialogue>();
            List<TextExporterCinematic> textExporterCinematics = new List<TextExporterCinematic>();
            List<TextExporterCinematic> textExporterLocalCinematics = new List<TextExporterCinematic>();
            List<TextExportObject> textExportInteractables = new List<TextExportObject>();
            List<TextExportObject> textExportDelegates = new List<TextExportObject>();

            SearchScene(scenePath, textExporterDialogues, textExporterCinematics, textExporterLocalCinematics, textExportInteractables, textExportDelegates);

            containers.Add(new SceneTextContainer(GetSceneNameFromPath(scenePath), textExporterDialogues, textExporterCinematics,
                textExporterLocalCinematics, textExportInteractables));

            //GetSceneNameFromPath(scenePath)
        }

        //Search data
        SearchData(textExportItems, textExportDialogueBatches);

        //Package everything
        AllTextContainer allData = new AllTextContainer(wordCount, new DataContainer(textExportItems, textExportDialogueBatches), menuContainers, containers);

        //Export to JSON
        var json = JsonUtility.ToJson(allData, true);

        File.WriteAllText(path, json);

        Debug.Log("Done!");
    }

    void SearchData(List<TextExportItem> textExportItems, List<TextExportDialogueBatch> textExportDialogueBatches)
    {
        //DirectoryInfo itemDir = new DirectoryInfo(itemsDirectory);


        //Get item files
        string[] interactionGUIDs = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/Prefabs/Data/ItemData", "Assets/Prefabs/Data/ItemData/Ceremony", "Assets/Prefabs/Data/ItemData/LongestDay" });
        foreach (var guid in interactionGUIDs)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid));

            ItemData itemData = (ItemData)asset;

            if (itemData != null)
            {
                List<string> description = new List<string>();

                foreach (var item in itemData.Inspect)
                {
                    description.Add(item.Text);

                    wordCount += CountWords(item.Text);
                }

                textExportItems.Add(new TextExportItem(itemData.Name, description.ToArray()));
            }
        }

        //Get dialogue batches
        interactionGUIDs = AssetDatabase.FindAssets("t:DialogueBatch", new[] { "Assets/Prefabs/MingleMinigame"});
        
        foreach (var guid in interactionGUIDs)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid));

            DialogueBatch batch = (DialogueBatch)asset;

            if (batch != null)
            {
                List<string> lines = new List<string>();

                foreach (var item in batch.lines)
                {
                    lines.Add(item);

                    wordCount += CountWords(item);
                }

                textExportDialogueBatches.Add(new TextExportDialogueBatch(batch.batchName, lines.ToArray()));
            }
        }
    }

    void SearchScene(string scenePath, List<TextExporterDialogue> textExporterDialogues, List<TextExporterCinematic> textExporterCinematics, 
        List<TextExporterCinematic> textExporterLocalCinematics, List<TextExportObject> textExportInteractables, List<TextExportObject> textExportDelegates)
    {
        //Open scene
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        //Search for everything
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        CinematicManager cinematicManager = FindObjectOfType<CinematicManager>();
        LocalCinematic[] localCinematics = FindObjectsOfType<LocalCinematic>();
        CommentInteractable[] commentInteractables = FindObjectsOfType<CommentInteractable>();
        Door[] doors = FindObjectsOfType<Door>();

        //Get delegate objects (DOESNT WORK FOR NOW)
        var Delegates = EventDependencyHunter.FindDependencies("WriteComment");

        foreach (var item in Delegates)
        {
            textExportDelegates.Add(new TextExportObject(item.Owner.gameObject.name, "text"));
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
    void SearchSceneAsMenu(string scenePath, List<TextExporterTextMeshPro> textExporterTextMeshPros)
    {
        //Open scene
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();

        foreach (var item in texts)
        {
            textExporterTextMeshPros.Add(new TextExporterTextMeshPro(item.gameObject.name, item.text));

            wordCount += CountWords(item.text);
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

    string GetSceneNameFromPath(string scenePath)
    {
        string sceneName = SceneManager.GetSceneByPath(scenePath).name;

        foreach (var c in scenePath)
        {
            sceneName += c;

            if (c == '/')
                sceneName = "";
        }

        string[] split = sceneName.Split('.');

        return split[0];
    }
}


[Serializable]
public class AllTextContainer
{
    public int globalWordCount;
    public DataContainer data;
    public MenuSceneTextContainer[] menuScenes;
    public SceneTextContainer[] scenes;

    public AllTextContainer(int global, DataContainer dataContainer, List<MenuSceneTextContainer> menuContainers, List<SceneTextContainer> containers)
    {
        globalWordCount = global;
        menuScenes = menuContainers.ToArray();
        scenes = containers.ToArray();
        data = dataContainer;
    }
}

[Serializable]
public class MenuSceneTextContainer
{
    public string sceneName;
    public TextExporterTextMeshPro[] textMeshPros;

    public MenuSceneTextContainer(string sceneName, List<TextExporterTextMeshPro> textMeshPros)
    {
        this.sceneName = sceneName;
        this.textMeshPros = textMeshPros.ToArray();
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
public class DataContainer
{
    public TextExportItem[] textExportItems;
    public TextExportDialogueBatch[] textExportDialogueBatches;

    public DataContainer(List<TextExportItem> textExportItems, List<TextExportDialogueBatch> textExportDialogueBatches)
    {
        this.textExportItems = textExportItems.ToArray();
        this.textExportDialogueBatches = textExportDialogueBatches.ToArray();
    }
}

[Serializable]
public class TextExporterTextMeshPro
{
    public string objectName;
    public string text;

    public TextExporterTextMeshPro(string objectName, string text)
    {
        this.objectName = objectName;
        this.text = text;
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

[Serializable]
public class TextExportItem
{
    public string ItemName;
    public string[] description;

    public TextExportItem(string o, string[] t)
    {
        ItemName = o;
        description = t;
    }
}

[Serializable]
public class TextExportDialogueBatch
{
    public string BatchName;
    public string[] description;

    public TextExportDialogueBatch(string o, string[] t)
    {
        BatchName = o;
        description = t;
    }
}

#endif
