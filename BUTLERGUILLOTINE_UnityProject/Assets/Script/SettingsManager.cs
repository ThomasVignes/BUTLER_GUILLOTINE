using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private PostProcessOnUI postProcessOnUI;
    [SerializeField] private TMP_Dropdown screen, sound, fps;
    [SerializeField] private Toggle vsync, postOnUI;
    [SerializeField] private Slider masterVolume, musicVolume, sfxVolume;

    public void Init()
    {
        var data = PersistentData.Instance;

        if (data == null)
            return;

        InitValues(data);
    }

    void InitValues(PersistentData data)
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                screen.value = 0;
                break;

            case FullScreenMode.Windowed:
                screen.value = 1;
                break;

            case FullScreenMode.ExclusiveFullScreen:
                screen.value = 2;
                break;
        }

        if (data.Stereo)
            MasterStereo();
        else
            MasterMono();

        switch (data.Framerate)
        {
            case 0:
                fps.value = 0;
                break;

            case 120:
                fps.value = 1;
                break;

            case 60:
                fps.value = 2;
                break;

            case 30:
                fps.value = 3;
                break;
        }

        vsync.isOn = data.Vsync;
        postOnUI.isOn = data.PostProcessAffectsUI;

        masterVolume.value = data.MasterVolume;
        musicVolume.value = data.MusicVolume;
        sfxVolume.value = data.SFXVolume;

        postProcessOnUI.Init();
    }

    public void UpdateScreen()
    {
        switch (screen.value)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;

            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        PersistentData.Instance.Screen = Screen.fullScreenMode;
    }

    public void UpdateSound()
    {
        switch(sound.value) 
        {
            case 0:
                MasterStereo();
                PersistentData.Instance.Stereo = true;
                break;

            case 1:
                MasterMono();
                PersistentData.Instance.Stereo = false;
                break;
        }
    }

    public void UpdateFps()
    {
        var targetFps = 0;

        switch (fps.value)
        {
            case 0:
                targetFps = 0; 
                break;

            case 1:
                targetFps = 120;
                break;

            case 2:
                targetFps = 60;
                break;

            case 3:
                targetFps = 30;
                break;
        }

        Application.targetFrameRate = targetFps;
        PersistentData.Instance.Framerate = targetFps;
    }

    public void UpdateVsync()
    {
        bool toggle = vsync.isOn;

        if (toggle)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;

        PersistentData.Instance.Vsync = toggle;
    }

    public void UpdatePostProcess()
    {
        postProcessOnUI.ToggleAllCanvases(postOnUI.isOn);
        PersistentData.Instance.PostProcessAffectsUI = postOnUI.isOn;
    }

    public void UpdateVolume()
    {
        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.MasterVolume = masterVolume.value;
            PersistentData.Instance.MusicVolume = musicVolume.value;
            PersistentData.Instance.SFXVolume = sfxVolume.value;
        }
    }



    void MasterMono()
    {
        var core = FMODUnity.RuntimeManager.CoreSystem;
        core.setSoftwareFormat(48000, FMOD.SPEAKERMODE.MONO, 0);
    }

    void MasterStereo()
    {
        var core = FMODUnity.RuntimeManager.CoreSystem;
        core.setSoftwareFormat(48000, FMOD.SPEAKERMODE.STEREO, 0);
    }
}
