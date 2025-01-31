using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableLock : Lifeform
{
    [SerializeField] Door[] doors;
    [SerializeField] GameObject lockMesh;
    public override void Death()
    {
        base.Death();

        foreach (var item in doors)
        {
            item.ToggleDoorNoEvent(true);
        }

        Instantiate(lockMesh, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
