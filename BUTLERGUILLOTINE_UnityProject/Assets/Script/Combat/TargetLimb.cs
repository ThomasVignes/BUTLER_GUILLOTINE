using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Whumpus;

public class TargetLimb : Hurtbox
{
    [Header("Limb Settings")]
    public Lifeform Owner;
    public int Multiplier = 1;
    public float StunMultiplier = 1;
    public float ForceMultiplier = 1;
    public RagdollLimb limb;

    [Header("Immunity")]
    public bool Immune;
    public string ImmuneMessage;

    LimbShield shield;

    public bool Shielded { get { if (shield == null) return false; else return shield.Active; } }

    private void Awake()
    {
        shield = GetComponent<LimbShield>();
    }

    public void Hit(int damage, float stun, float force, Vector3 dir)
    {
        if (limb != null)
            Debug.Log(limb.gameObject.name + " hit");
        

        if (shield == null || !shield.Active)
        {
            Owner.Hurt(damage * Multiplier);
            Owner.Stun(stun * StunMultiplier);

            if (limb != null)
                limb.rb.AddForce(force * dir);
        }
        else
        {
            shield.Absorb(damage * Multiplier);
        }

        Hit();
    }

    public void MakeVulnerable()
    {
        Immune = false;
    }
}
