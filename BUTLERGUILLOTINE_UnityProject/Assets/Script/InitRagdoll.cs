using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Whumpus;

public class InitRagdoll : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool tryMatchBones;
    [SerializeField] bool matchGlobal;
    [SerializeField] bool matchAllLimbs;

    [Header("Freeze")]
    [SerializeField] bool freezeAfterDelay;
    [SerializeField] float secondsBeforeFreeze;

    [Header("Utility")]
    public Rigidbody torsoRb;

    bool done;
    float timer;

    private void Awake()
    {
        timer = Time.time + secondsBeforeFreeze;
    }

    void Update()
    {
        if (done)
            return;

        if (!freezeAfterDelay)
            return;

        if (timer < Time.time)
        {
            done = true;
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();

            foreach (var item in bodies)
            {
                item.isKinematic = true;
            }
        }
    }

    public void TryMatchBones(Transform parent)
    {
        if (!tryMatchBones)
            return;

        Transform[] bones = transform.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform item in parent.GetComponentsInChildren<Transform>())
        {
            if (item.GetComponent<RagdollLimb>() || matchAllLimbs)
            {
                foreach (var bone in bones)
                {
                    if (bone.name == item.name)
                    {
                        if (!matchGlobal)
                        {
                            bone.localPosition = item.localPosition;
                            bone.localRotation = item.localRotation;
                        }
                        else
                        {
                            bone.position = item.position;
                            bone.rotation = item.rotation;
                        }
                    }
                }
            }
        }
    }

    public void Redirect(Vector3 dir, float force)
    {
        foreach (Transform item in transform.gameObject.GetComponentsInChildren<Transform>())
        {
            var rb = item.GetComponent<Rigidbody>();

            if (rb)
                rb.velocity = Vector3.zero;
        }

        torsoRb.AddForce(dir * force);
    }
}
