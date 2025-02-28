using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoldInteractable : Interactable
{
    [Header("Hold settings")]
    public UnityEvent OnUnHold;
    public bool Hold;

    protected Character holder;


    private void Update()
    {
        Step();
    }

    protected virtual void Step()
    {
        if (Hold && holder.Moving)
            LeaveEffects();
    }

    protected override void InteractEffects(Character character)
    {
        base.InteractEffects(character);

        Hold = true;
        holder = character;
    }

    protected virtual void LeaveEffects()
    {
        Hold = false;
        holder = null;

        OnUnHold?.Invoke();
    }
}
