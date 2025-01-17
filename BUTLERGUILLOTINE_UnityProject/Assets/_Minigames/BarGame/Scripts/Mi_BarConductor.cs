using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarConductor : MonoBehaviour
{
    public static Mi_BarConductor Instance;
    public float songBpm;

    public float secPerBeat;

    public float songPosition;

    public float songPositionInBeats;

    public float dspSongTime;

    public float firstBeatOffset;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);

        songPositionInBeats = songPosition / secPerBeat;
    }

    public void Ready()
    {
        secPerBeat = 60f / songBpm;

        dspSongTime = (float)AudioSettings.dspTime;
    }
}