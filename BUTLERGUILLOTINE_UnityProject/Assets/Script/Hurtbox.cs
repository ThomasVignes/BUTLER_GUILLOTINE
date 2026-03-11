using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Hurtbox : MonoBehaviour
{
    [Header("Hurtbox Settings")]
    public bool NoBlood;
    public UnityEvent OnHit;

    public virtual void Hit()
    {
        OnHit.Invoke();
    }
}
