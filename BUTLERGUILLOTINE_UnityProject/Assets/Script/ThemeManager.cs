using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ThemeManager : MonoBehaviour
{
    public bool ClearInterScenes = true;
    [SerializeField] MusicData[] themes;
    [SerializeField] List<Area> areas = new List<Area>();
    [SerializeField] AudioSource overrideAudio, startAudio;
    [SerializeField] string startAreaExperimental;

    string currentArea;
    float currentVolume;
    AudioSource currentAudioSource;

    string currentOverride;
    bool overrideAmbiance;
    float overrideTime;

    EventInstance currentInstance, currentOverrideInstance;

    public void Init()
    {
        if (ClearInterScenes)
            ButlerEngineUtilities.ClearAllInterScenes();

        areas.Clear();

        foreach (var theme in themes)
        {
            areas.Add(new Area(theme.Name, theme.Track, theme.StepsVolume, theme.ImmuneExperimental));
        }

        foreach (var area in areas)
        {
            area.Init();
        }

        if (startAudio != null)
            startAudio.Play();
        else if (startAreaExperimental != "")
            NewArea(startAreaExperimental);
    }

    void SwapEvent(EventReference track)
    {
        currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        currentInstance = RuntimeManager.CreateInstance(track);
        //instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        currentInstance.start();
        currentInstance.release();
    }

    void SwapOverride(EventReference track)
    {
        PersistentData.Instance.MusicMultiplier = 1;

        currentOverrideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        currentOverrideInstance = RuntimeManager.CreateInstance(track);
        //instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        currentOverrideInstance.start();
        currentOverrideInstance.release();
    }

    public void NewArea(string areaName)
    {
        if (overrideAmbiance)
            return;

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                PersistentData.Instance.MusicMultiplier = 1;
                PersistentData.Instance.UpdateStepsVolume(item.StepsVolume);

                if (item.Name != currentArea)
                {
                    SwapEvent(item.Track);

                    currentArea = item.Name;
                }
            }
        }
    }

    public void NewArea(string areaName, float volume)
    {
        if (overrideAmbiance)
            return;

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                PersistentData.Instance.MusicMultiplier = volume;
                PersistentData.Instance.UpdateStepsVolume(item.StepsVolume);

                if (item.Name != currentArea)
                {
                    SwapEvent(item.Track);

                    currentArea = item.Name;
                }
            }
        }
    }

    public void ResumeAmbiance()
    {
        NewArea(currentArea);
    }

    public void StopAmbiance()
    {
        currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //currentOverrideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void PlayEndAmbiance()
    {
        overrideAmbiance = true;

        overrideAudio.Play();
    }

    public void CreateInterScene(string areaName)
    {
        StopAmbiance();

        overrideAmbiance = true;
        PersistentData.Instance.UpdateStepsVolume(0);

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                var interScene = Instantiate((GameObject)Resources.Load("GameManagement/InterSceneTheme"));
                InterSceneTheme theme = interScene.GetComponent<InterSceneTheme>();

                theme.Play(item.Track);
            }
        }
    }

    public void OverrideAmbiance(string areaName)
    {
        StopAmbiance();

        overrideAmbiance = true;
        PersistentData.Instance.UpdateStepsVolume(0);

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                currentOverride = areaName;

                SwapOverride(item.Track);
            }
        }
    }

    public void StopOverride()
    {
        bool same = currentOverride == currentArea;

        int pos = 0;
        currentOverrideInstance.getTimelinePosition(out pos);

        currentOverrideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        overrideAmbiance = false;

        foreach (var item in areas)
        {
            if (item.Name == currentArea)
            {
                SwapEvent(item.Track);

                currentArea = item.Name;

                if (same)
                    currentInstance.setTimelinePosition(pos);
            }
        }
    }

    public void StopOverride(string resumeTheme)
    {
        bool same = currentOverride == resumeTheme;


        int pos = 0;
        currentOverrideInstance.getTimelinePosition(out pos);

        currentOverrideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        overrideAmbiance = false;

        foreach (var item in areas)
        {
            if (item.Name == resumeTheme)
            {
                SwapEvent(item.Track);

                currentArea = item.Name;

                if (same)
                    currentInstance.setTimelinePosition(pos);
            }
        }

    }

    public void SetAmbianceVolume(float sound)
    {
        /*
        currentOverrideInstance.setVolume(sound);
        currentInstance.setVolume(sound);
        */

        //Modify once parameters are set

        if (currentAudioSource != null)
            currentAudioSource.volume = currentVolume * sound;

        if (overrideAmbiance)
            overrideAudio.volume = currentVolume * sound;
    }

    private void OnDestroy()
    {
        currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        currentOverrideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
