using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarClickableCollider : MonoBehaviour
{
    public Action Triggered;

    private void OnMouseEnter()
    {
        Triggered?.Invoke();
    }
}
