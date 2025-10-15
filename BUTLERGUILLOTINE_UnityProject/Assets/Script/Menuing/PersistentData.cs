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

    [Header("Settings")]
    public bool FullScreen;
    public bool SoundOn;

    [HideInInspector] public bool FastMode, CopyrightFree;

    [Header("Demo Specific Data")]
    public bool FinishedOnce;
    public bool HasKey;

    [Header("Saving")]
    public string savePath = @"\Resources\Saves\";

    [HideInInspector] public BuildNavigator BuildNavigator;

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
#else
        LoadData();
#endif

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
            Destroy(gameObject);

        BuildNavigator = GetComponent<BuildNavigator>();
        BuildNavigator.Init(this);

        FullScreen = true;
        SoundOn = true;

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

    //Saving
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        //int increment = 0;

        CurrentScene = SceneManager.GetActiveScene().name;

        var data = new SaveData(CurrentScene, FullScreen, SoundOn, FinishedOnce, HasKey);
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

        FullScreen = saveData.Settings.FullScreen;
        SoundOn = saveData.Settings.SoundOn;

        FinishedOnce = saveData.DemoTriggers.FinishedOnce;
        HasKey = saveData.DemoTriggers.HasKey;
    }

    public void SaveData(bool noScene)
    {
        //int increment = 0;

        if (noScene)
            CurrentScene = "";
        else
            CurrentScene = SceneManager.GetActiveScene().name;

        var data = new SaveData(CurrentScene, FullScreen, SoundOn, FinishedOnce, HasKey);
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

    public SaveData(string currentScene, bool fullScreen, bool soundOn, bool finishedOnce, bool hasKey)
    {
        GeneralData = new GeneralData();
        GeneralData.CurrentScene = currentScene;

        Settings = new Settings();
        Settings.FullScreen = fullScreen;
        Settings.SoundOn = soundOn;

        DemoTriggers = new DemoTriggers();
        DemoTriggers.FinishedOnce = finishedOnce;
        DemoTriggers.HasKey = hasKey;
    }
}

[System.Serializable]
public class Settings
{
    public bool FullScreen;
    public bool SoundOn;
}

[System.Serializable]
public class GeneralData
{
    public string CurrentScene;
}


[System.Serializable]
public class DemoTriggers
{
    public bool FinishedOnce;
    public bool HasKey;
}
