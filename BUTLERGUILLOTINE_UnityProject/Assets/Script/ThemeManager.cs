using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class ThemeManager : MonoBehaviour
{
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
        areas.Clear();

        foreach (var theme in themes)
        {
            areas.Add(new Area(theme.Name, theme.Track, theme.ImmuneExperimental));
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

        /*
        emitter.Stop();
        emitter.EventReference = track;
        emitter.Play();
        */
    }

    void SwapOverride(EventReference track)
    {        
        currentOverrideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        currentOverrideInstance = RuntimeManager.CreateInstance(track);
        //instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        currentOverrideInstance.start();
        currentOverrideInstance.release();

        /*
        overrideEmitter.Stop();
        overrideEmitter.EventReference = track;
        overrideEmitter.Play();
        */
    }

    public void NewArea(string areaName)
    {
        if (overrideAmbiance)
            return;

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                //Change emitter volume here once parameters are figured out

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

    public void OverrideAmbiance(string areaName)
    {
        StopAmbiance();

        overrideAmbiance = true;

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
