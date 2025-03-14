using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableLock : Lifeform
{
    [SerializeField] string overrideMessage = "I could shoot the lock open.";
    [SerializeField] Door[] doors;
    [SerializeField] GameObject lockMesh;

    private void Start()
    {
        if (overrideMessage == "")
            return;

        foreach (Door door in doors) 
        {
            door.LockedMessage = overrideMessage;
        }
    }

    public override void Death()
    {
        base.Death();

        foreach (var item in doors)
        {
            item.ToggleDoorNoEvent(true);
        }

        GameObject go = Instantiate(lockMesh, transform.position, transform.rotation);

        go.GetComponent<Rigidbody>().AddForce(80 * -transform.right.normalized + 30 * -Vector3.up);

        Destroy(gameObject);
    }
}
