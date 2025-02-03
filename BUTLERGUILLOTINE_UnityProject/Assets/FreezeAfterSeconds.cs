using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAfterSeconds : MonoBehaviour
{
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
}
