using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : HoldInteractable
{
    public bool Available = true;

    private void Update()
    {
        Step();
    }

    protected override void InteractEffects(Character character)
    {
        base.InteractEffects(character);

        Available = false;
    }

    protected override void LeaveEffects()
    {
        base.LeaveEffects();

        Available = true;
    }
}
