using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public bool FullScreen;
    public bool SoundOn;
    public bool CopyrightFree;
    public bool FastMode;
    public bool DemoMode;

    [Header("Demo Specific")]
    public bool FinishedOnce;
    public bool HasKey;

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
}
