using UnityEngine.Audio;
using UnityEngine;
using FMODUnity;

[System.Serializable]
public class Sound
{
    public string name;

    public EventReference clipReference;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    public string mixerLayer;

    [HideInInspector]
    public StudioEventEmitter audioEmitter;
    [HideInInspector]
    public AudioSource source;
}
