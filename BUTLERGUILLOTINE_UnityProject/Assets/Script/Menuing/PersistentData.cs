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

    string masterBusPath = "bus:/";
    string musicBusPath = "bus:/Music";
    string sfxBusPath = "bus:/SFX";

    FMOD.Studio.Bus masterBus, musicBus, sfxBus;

    public float MasterVolume { get { return master; } set { master = value; UpdateVolumes(); } }
    public float MusicVolume {  get { return music; } set { music = value; UpdateVolumes(); } }
    public float SFXVolume { get { return sfx; } set { sfx = value; UpdateVolumes(); } }

    float master = 1, music = 1, sfx = 1;

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

        UpdateVolumes();
    }

    public void UpdateVolumes()
    {
        masterBus.setVolume(master);
        musicBus.setVolume(music);
        sfxBus.setVolume(sfx);
    }
}
