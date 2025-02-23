using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioObject : MonoBehaviour
{
    [SerializeField] StudioEventEmitter emitter;
    [SerializeField] private AudioSource audioSource;

    //public Action SoundDone;

    /*
    public IEnumerator SoundPlayed()
    {
        int length = 0;

        emitter.EventDescription.getLength(out length);
        yield return new WaitForSeconds(audioSource.clip.length);

        if (audioSource.loop == false)
        {
            SoundDone?.Invoke();
        }
    }
    */
}
