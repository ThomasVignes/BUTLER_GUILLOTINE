using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whumpus;

public class StepSFX : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] StudioEventEmitter Hard;
    [SerializeField] StudioEventEmitter Fabric;
    [SerializeField] StudioEventEmitter Flesh;

    [Header("Tags")]
    [SerializeField] string HardTag;
    [SerializeField] string FabricTag;
    [SerializeField] string FleshTag;

    [Header("ScreenDetection")]
    [SerializeField] float lenience = 0.1f;

    EventInstance currentInstance;

    public void StepSound()
    {
        if (!WhumpusUtilities.IsInScreen(transform, lenience))
            return;

        Ray ray = new Ray(transform.position, -Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            ProcessTag(hit.transform.gameObject.tag);
        }
    }

    void ProcessTag(string tag)
    {
        if (tag == HardTag)
            PlaySound(Hard);
        else if (tag == FabricTag)
            PlaySound(Fabric);
        else if (tag == FleshTag)
            PlaySound(Flesh);
        else
            PlaySound(Fabric);
    }

    void PlaySound(StudioEventEmitter emitter)
    {
        emitter.Play();
    }
}
