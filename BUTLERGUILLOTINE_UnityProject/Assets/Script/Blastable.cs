using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blastable : Lifeform
{
    [Header("Blastable butler specifics")]
    public bool changeBowtie;
    //[SerializeField] GameObject[] objectsToSave;

    public override void Death()
    {
        /*
        foreach (var item in objectsToSave)
        {
            item.transform.SetParent(null);
            var rb = item.AddComponent<Rigidbody>();
            item.AddComponent<FreezeRb>();

            rb.AddForce(Vector3.up * 70f);
        }
        */

        base.Death();
    }
}
