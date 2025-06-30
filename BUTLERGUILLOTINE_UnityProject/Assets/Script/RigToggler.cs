using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigToggler : MonoBehaviour
{
    [SerializeField] Rig rig;

    float interpSpeed = 4;
    float targetWeight;

    public void ToggleRig(bool active)
    {
        if (active)
            targetWeight = 1;
        else
            targetWeight = 0;
    }

    private void Update()
    {
        if (Mathf.Abs(rig.weight - targetWeight) < 0.02)
            return;

        if (rig.weight > targetWeight)
            rig.weight -= interpSpeed/2 * Time.deltaTime;
        else
            rig.weight += interpSpeed * Time.deltaTime;

    }
}
