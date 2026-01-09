using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whumpus;

public class Hitshape : MonoBehaviour
{
    [SerializeField] Lifeform owner;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] int damage;
    [SerializeField] int stun;
    [SerializeField] float force;
    [SerializeField] float linger;
    [SerializeField] GameObject hitFx;
    [SerializeField] string hitSound;
    public bool projectile;
    public InitRagdoll linkedRagdoll;

    bool active;
    float lingerTimer;
    Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();

        if (projectile)
            Trigger();
        else
            Disable();
    }

    private void Update()
    {
        if (projectile)
            return;

        if (active && lingerTimer < Time.time)
            Disable();
    }

    public void Trigger()
    {
        active = true;
        col.enabled = true;

        lingerTimer = Time.time + linger;
    }

    public void Disable()
    {
        col.enabled = false;
        active = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == WhumpusUtilities.ToLayer(hitLayer))
        {
            TargetLimb targetLimb = other.gameObject.GetComponent<TargetLimb>();

            if (targetLimb != null && targetLimb.Owner != owner)
            {
                Vector3 dir = transform.forward.normalized;

                if (!projectile)
                    dir = owner.transform.forward.normalized;

                targetLimb.Hit(damage, stun, force, dir);

                Instantiate(hitFx, other.ClosestPoint(transform.position), Quaternion.identity);

                if (projectile && linkedRagdoll != null)
                {
                    linkedRagdoll.Redirect(Vector3.Normalize(transform.position - targetLimb.Owner.transform.position), 3000);
                }

                if (hitSound != null) 
                    EffectsManager.Instance.audioManager.Play(hitSound);
                
                if (!projectile)
                    Disable();
            }
        }
    }
}
