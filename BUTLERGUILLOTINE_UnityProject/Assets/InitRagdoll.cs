using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Whumpus;

public class InitRagdoll : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool tryMatchBones;

    [Header("Freeze")]
    [SerializeField] bool freezeAfterDelay;
    [SerializeField] float secondsBeforeFreeze;

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
            if (item.GetComponent<RagdollLimb>())
            {
                foreach (var bone in bones)
                {
                    if (bone.name == item.name)
                    {
                        bone.localRotation = item.localRotation;
                    }
                }
            }
        }
    }
}
