using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Music")]
public class MusicData : ScriptableObject
{
    [Header("Data")]
    public string Name;
    public EventReference Track;

    [Header("Settings")]
    public float StepsVolume = 1;
    public bool ImmuneExperimental;
}
