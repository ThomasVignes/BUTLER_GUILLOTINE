using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Whumpus;

public class FootManager : MonoBehaviour
{
    [Header("Settings")]
    public bool CanSound;
    public bool CanVfx;
    [SerializeField] float delayBetweenSteps;
    public LayerMask Ground;

    [Header("References")]
    [SerializeField] StudioEventEmitter emitter;
    [SerializeField] private GameObject walkParticle;
    [SerializeField] private Transform particleSpot;


    float timer;
    private GameObject currentVfx;
   

    private void OnCollisionEnter(Collision collision)
    {
        int[] layers = { 0, 4, 6, 9 };

        if (layers.Contains(collision.gameObject.layer) && timer < Time.time)
        {
            emitter.Stop();
            emitter.Play();

            if (CanVfx && currentVfx == null && walkParticle != null)
            {
                currentVfx = Instantiate(walkParticle);
                currentVfx.transform.position = particleSpot.position;
            }

            timer = Time.time + delayBetweenSteps;
        }
    }
}
