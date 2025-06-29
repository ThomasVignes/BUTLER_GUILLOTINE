using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ProximitySound : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float maxRange;
    [SerializeField] float minRange;
    [SerializeField] AnimationCurve falloff;

    [Header("References")]
    [SerializeField] StudioEventEmitter emitter;

    [Header("Editor")]
    [SerializeField] bool showRanges;

    Transform playerPos;

    bool running;

    private void Update()
    {
        if (running)
            Step();
    }

    public void SetActive(bool active)
    {
        running = active;

        if (active)
        {
            if (playerPos == null)
                Init();

            emitter.Play();
            emitter.EventInstance.setVolume(0);
        }
        else
        {
            emitter.Stop();
        }
    }

    void Init()
    {
        playerPos = GameManager.Instance.PlayerFollower.transform;
    }

    void Step()
    {
        var dist = Vector3.Distance(transform.position, playerPos.position);

        var volume = 1 - dist / maxRange;

        if (volume < 0)
            volume = 0;

        if (dist < minRange) 
            volume = 1;

        emitter.EventInstance.setVolume(volume);
    }

    private void OnDrawGizmos()
    {
        if (!showRanges)
            return;

        Gizmos.color = Color.red; 

        Gizmos.DrawWireSphere(transform.position, maxRange);


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);
    }
}
