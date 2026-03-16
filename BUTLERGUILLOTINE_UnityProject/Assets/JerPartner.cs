using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.Text;

public class JerPartner : Character
{
    [Header("Vision")]
    [SerializeField] float targetRange;
    [SerializeField] LayerMask targetLayer;

    [Header("Gunplay")]
    [SerializeField] float shootCD, drawDelay;
    [SerializeField] Rig legIK;
    [SerializeField] StudioEventEmitter gunshotEmitter;
    [SerializeField] GameObject hipGun, handGun, muzzle;
    [SerializeField] GameObject shootParticle, bloodParticle, muzzleParticle, bloodDecal;

    LayerMask ignoreLayers;
    bool Aiming, RigOn;
    float shootTimer;
    Lifeform lifeform;

    Lifeform targetEnemy;

    public override void Init()
    {
        base.Init();

        ignoreLayers = GameManager.Instance.IgnoreLayers;

        legIK.weight = 0;

        lifeform = GetComponent<Lifeform>();

        DrawGun(false);
    }

    public override void Step()
    {
        if (targetEnemy == null)
        {
            if (Aiming)
                DrawGun(false);

            base.Step();

            Collider[] targets = Physics.OverlapSphere(transform.position, targetRange, targetLayer);

            if (targets.Length > 0)
            {
                var targ = GetClosestEnemy(targets);

                if (targ != null) 
                    targetEnemy = targ.Owner;
            }

            if (targetEnemy != null)
                DrawGun(true);
        }
        else
        {
            PausePath();

            if (Aiming)
            {
                Vector3 targPos = targetEnemy.transform.position;

                targetDir = Vector3.Normalize(targPos - transform.position);

                transform.forward = Vector3.Lerp(transform.forward, targetDir, rotationSpeed * Time.deltaTime);

                RaycastHit hit;
                Ray ray = new Ray(transform.position + Vector3.up * 1, Vector3.Normalize(targetEnemy.transform.position - transform.position));

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
                    Shoot(targetEnemy.transform.position + Vector3.up * 1, hit.transform.gameObject);
            }
        }

        if (RigOn)
            legIK.weight = Mathf.Lerp(legIK.weight, 1, Time.deltaTime * 4);
        else
            legIK.weight = Mathf.Lerp(legIK.weight, 0, Time.deltaTime * 4);
    }

    public void SearchTarget()
    {

    }

    public void DrawGun(bool active)
    {
        base.ToggleSpecial(active);

        Aiming = active;

        hipGun.SetActive(!active);
        handGun.SetActive(active);

        if (Aiming)
        {
            shootTimer = Time.time + drawDelay;
        }

        computing = false;
        agent.isStopped = active;

        if (active)
        {
            PausePath();
        }
        else
        {
            agent.ResetPath();
        }

        animator.SetBool("Aim", active);
        RigOn = active;
    }

    public void Shoot(Vector3 spot, GameObject hitObject)
    {
        if (stunned)
            return;

        if (shootTimer > Time.time)
            return;

        shootTimer = Time.time + shootCD;

        base.Special(spot, hitObject);

        animator.SetTrigger("Shoot");

        gunshotEmitter.Play();

        GameManager.Instance.HitstopManager.StartHitstop();

        Hurtbox hurtbox = hitObject.GetComponent<Hurtbox>();
        TargetLimb targetLimb = hitObject.GetComponent<TargetLimb>();

        GameObject go = Instantiate(muzzleParticle, muzzle.transform.position, muzzle.transform.rotation);

        if (targetLimb != null)
            HitLimb(spot, hitObject, targetLimb);
        else if (hurtbox != null)
            HitHurtbox(spot, hurtbox);

    }

    private void HitHurtbox(Vector3 spot, Hurtbox hurtbox)
    {
        hurtbox.Hit();

        GameObject particle = null;

        if (hurtbox.NoBlood)
        {
            particle = Instantiate(shootParticle, spot, Quaternion.identity);
            EffectsManager.Instance.audioManager.Play("Surface");
        }
        else
        {
            particle = Instantiate(bloodParticle, spot, Quaternion.identity);
            EffectsManager.Instance.audioManager.Play("Blood");
        }

        particle.transform.LookAt(transform.position + Vector3.up * 2f);
    }

    private void HitLimb(Vector3 spot, GameObject hitObject, TargetLimb targetLimb)
    {
        var shotLifeform = false;
        var shielded = false;
        GameObject particle = null;

        if (targetLimb != null && targetLimb.Owner != lifeform)
        {
            targetLimb.Hit(1, 1, 2500, transform.forward.normalized);

            if (!targetLimb.Shielded)
            {
                GameObject blood = Instantiate(bloodDecal, hitObject.transform);
                blood.transform.position = spot;
                blood.transform.forward = transform.forward;
            }

            shielded = targetLimb.Shielded;
            shotLifeform = true;
        }

        if (shielded)
        {
            particle = Instantiate(shootParticle, spot, Quaternion.identity);
            EffectsManager.Instance.audioManager.Play("Shield");
        }
        else
        {
            if (shotLifeform)
            {
                if (targetLimb.NoBlood)
                {
                    particle = Instantiate(shootParticle, spot, Quaternion.identity);
                    EffectsManager.Instance.audioManager.Play("Shield");
                }
                else
                {
                    particle = Instantiate(bloodParticle, spot, Quaternion.identity);
                    EffectsManager.Instance.audioManager.Play("Blood");
                }

                particle.transform.LookAt(transform.position + Vector3.up * 2f);


            }
            else
            {
                particle = Instantiate(shootParticle, spot, Quaternion.identity);
                EffectsManager.Instance.audioManager.Play("Surface");
            }
        }
    }

    TargetLimb GetClosestEnemy(Collider[] enemies)
    {
        TargetLimb tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Collider t in enemies)
        {
            var targetLimb = t.gameObject.GetComponent<TargetLimb>();
            if (targetLimb != null)
            {
                float dist = Vector3.Distance(t.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = targetLimb;
                    minDist = dist;
                }
            }
        }

        return tMin;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }
}
