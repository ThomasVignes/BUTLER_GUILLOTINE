using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whumpus;

public class Test : MonoBehaviour
{
    [SerializeField] DiversuitRagdoll ragdoll;
    [SerializeField] Transform pos;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Tp();

        if (Input.GetKeyDown(KeyCode.O))
            Freeze();
    }


    [ContextMenu("Tp")]
    public void Tp()
    {
        GameManager.Instance.TeleportPlayer(pos);
    }

    public void Freeze()
    {
        ragdoll.SetKinematic(true);
    }
}
