using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPlayerPos : MonoBehaviour
{
    [SerializeField] bool targetHead;

    bool Active;
    Transform targ;


    public void ToggleActive(bool active)
    {
        Active = active;
    }

    private void Update()
    {
        if (!Active)
            return;

        if (targ == null)
        {
            if (targetHead)
                targ = GameManager.Instance.PlayerFollower.Head;
            else
                targ = GameManager.Instance.PlayerFollower.transform;
        }
        else
            transform.position = targ.position;
    }
}
