using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public bool SaveInEditor;
    public bool DemoMode;

    [Header("Data")]
    public string CurrentScene;
    public int CurrentSceneIndex;

    [Header("Settings")]
    public FullScreenMode Screen;
    public bool Stereo = true;
    public int Framerate = 0;
    public bool Vsync = true;
    public bool PostProcessAffectsUI = true;

    [HideInInspector] public bool FastMode, CopyrightFree;

    [Header("Demo Specific Data")]
    public bool FinishedOnce;
    public bool HasKey;

    [Header("Saving")]
    string savePath = @"\Resources\";

    [HideInInspector] public BuildNavigator BuildNavigator;
    [HideInInspector] public SteamAchievementManager SteamAchievementManager;

    string masterBusPath = "bus:/";
    string musicBusPath = "bus:/Music";
    string sfxBusPath = "bus:/SFX";
    string stepsBusPath = "bus:/SFX/Steps";

    FMOD.Studio.Bus masterBus, musicBus, sfxBus, stepsBus;

    public float MasterVolume { get { return master; } set { master = value; UpdateVolumes(); } }
    public float MusicVolume {  get { return music; } set { music = value; UpdateVolumes(); } }
    public float SFXVolume { get { return sfx; } set { sfx = value; UpdateVolumes(); } }

    float master = 0.5f, music = 1, sfx = 1;

    public float MasterMultiplier {  get { return masterMultiplier; } set { masterMultiplier = value; UpdateVolumes(); } }
    public float MusicMultiplier { get { return musicMultiplier; } set { musicMultiplier = value; UpdateVolumes(); } }
    public float SfxMultiplier {  get { return sfxMultiplier; } set { sfxMultiplier = value; UpdateVolumes(); } }

    float masterMultiplier = 1, musicMultiplier = 1, sfxMultiplier = 1;

    public float Volume {  get { return AudioListener.volume; } set { AudioListener.volume = value; } }

    bool init;

    private void Awake()
    {
#if UNITY_EDITOR
        if (SaveInEditor)
            LoadData();

        QuickInit();

        return;
#endif

        LoadData();
        QuickInit();
    }

    public void QuickInit()
    {
        if (init)
            return;

        init = true;

        DontDestroyOnLoad(gameObject);

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        BuildNavigator = GetComponent<BuildNavigator>();
        BuildNavigator.Init(this);

        SteamAchievementManager = GetComponent<SteamAchievementManager>();  

        masterBus = FMODUnity.RuntimeManager.GetBus(masterBusPath);
        musicBus = FMODUnity.RuntimeManager.GetBus(musicBusPath);
        sfxBus = FMODUnity.RuntimeManager.GetBus(sfxBusPath);
        stepsBus = FMODUnity.RuntimeManager.GetBus(stepsBusPath);

        UpdateVolumes();
    }

    public void UpdateStepsVolume(float volume)
    {
        stepsBus.setVolume(volume);
    }

    public void UpdateVolumes()
    {
        masterBus.setVolume(master * masterMultiplier);
        musicBus.setVolume(music * musicMultiplier);
        sfxBus.setVolume(sfx * sfxMultiplier);
    }

    public void ResetMultipliers()
    {
        masterMultiplier = 1;
        musicMultiplier = 1;
        sfxMultiplier = 1;
    }

    public void ResetProgress()
    {
        CurrentScene = "";
        CurrentSceneIndex = 0;
        FinishedOnce = false;
        HasKey = false;

        SaveData(true);
    }

    //Saving
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        SaveData(true);
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        string fullPath = Application.dataPath + savePath + "SaveData";

        if (!File.Exists(fullPath + ".txt"))
        {
            SaveData(true);
            Debug.Log("Save not found, creating new save at " + savePath);

            return;
        }

        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(fullPath + ".txt"));

        ApplyData(saveData);

        Debug.Log("Applied " + "SaveData" + ".txt " + " at " + savePath);
    }

    public void ApplyData(SaveData saveData)
    {
        CurrentScene = saveData.GeneralData.CurrentScene;
        CurrentSceneIndex = saveData.GeneralData.CurrentSceneIndex;

        FinishedOnce = saveData.DemoTriggers.FinishedOnce;
        HasKey = saveData.DemoTriggers.HasKey;

        Settings settings = saveData.Settings;

        Screen = settings.Screen;
        Stereo = settings.Stereo;
        Framerate = settings.Framerate;
        Vsync = settings.Vsync;
        PostProcessAffectsUI = settings.PostProcessAffectsUI;
    }

    public void SaveData(bool saveCurrentScene)
    {
        //int increment = 0;

        if (saveCurrentScene)
        {
            CurrentScene = SceneManager.GetActiveScene().name;
            CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }

        var data = new SaveData(CurrentScene, CurrentSceneIndex, FinishedOnce, HasKey, Screen, Stereo, Framerate, Vsync, PostProcessAffectsUI);
        var json = JsonUtility.ToJson(data, true);

        string fullPath = Application.dataPath + savePath + "SaveData";

        /*
        while (File.Exists(fullPath + ".txt"))
        {
            increment++;
            fullPath = Application.dataPath + savePath + "SaveData";
        }
        */

        File.WriteAllText(fullPath + ".txt", json);

        Debug.Log("Created " + "SaveData" + ".txt " + " at " + savePath);
    }
}

[System.Serializable]
public class SaveData
{
    public GeneralData GeneralData;
    public Settings Settings;
    public DemoTriggers DemoTriggers;

    public SaveData(string currentScene, int currentSceneIndex, bool finishedOnce, bool hasKey, FullScreenMode screen, bool stereo, int framerate, bool vsync,bool postprocessui)
    {
        GeneralData = new GeneralData();
        GeneralData.CurrentScene = currentScene;
        GeneralData.CurrentSceneIndex = currentSceneIndex;

        DemoTriggers = new DemoTriggers();
        DemoTriggers.FinishedOnce = finishedOnce;
        DemoTriggers.HasKey = hasKey;

        Settings = new Settings();

        Settings.Screen = screen;
        Settings.Stereo = stereo;
        Settings.Framerate = framerate;
        Settings.Vsync = vsync;
        Settings.PostProcessAffectsUI = postprocessui;
    }
}

[System.Serializable]
public class Settings
{
    public FullScreenMode Screen;
    public bool Stereo = true;
    public int Framerate = 0;
    public bool Vsync = true;
    public bool PostProcessAffectsUI = true;
}

[System.Serializable]
public class GeneralData
{
    public string CurrentScene;
    public int CurrentSceneIndex;
}


[System.Serializable]
public class DemoTriggers
{
    public bool FinishedOnce;
    public bool HasKey;
}
