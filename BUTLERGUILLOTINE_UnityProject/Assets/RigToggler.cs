using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigToggler : MonoBehaviour
{
    [SerializeField] Rig rig;

    public void ToggleRig(bool active)
    {
        if (active)
            rig.weight = 1;
        else
            rig.weight = 0;
    }
}
