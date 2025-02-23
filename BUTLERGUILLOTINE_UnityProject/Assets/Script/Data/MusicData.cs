using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Music")]
public class MusicData : ScriptableObject
{
    public string Name;
    public EventReference Track;

    public bool ImmuneExperimental;
}
