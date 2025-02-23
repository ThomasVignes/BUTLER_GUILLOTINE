using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarMusicManager : MonoBehaviour
{
    public string[] TrackNames;
    private GameObject Player;
    private float nextBeat;
    private bool requestChange;
    private int currentTrack = 0;

    private void Start()
    {
        Mi_BarConductor.Instance.Ready();
        nextBeat = Mi_BarConductor.Instance.secPerBeat;
        //EffectsManager.Instance.audioManager.PlayTrack(TrackNames[currentTrack], false);
    }

    private void Update()
    {
        if (requestChange)
        {
            if (Mi_BarConductor.Instance.songPosition >= nextBeat)
            {
                //EffectsManager.Instance.audioManager.PlayTrack(TrackNames[currentTrack], true);
                Debug.Log("yes");
                requestChange = false;
            }
        }

        if (Mi_BarConductor.Instance.songPosition >= nextBeat)
        {
            nextBeat += Mi_BarConductor.Instance.secPerBeat;
        }
    }
}
