using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHoldInteractable : HoldInteractable
{
    [Header("References")]
    [SerializeField] List<Door> doors = new List<Door>();

    private void Update()
    {
        Step();
    }

    protected override void Step()
    {
        base.Step();
    }

    protected override void InteractEffects(Character character)
    {
        base.InteractEffects(character);

        foreach (var door in doors)
        {
            door.ToggleDoor(true);
        }
    }

    protected override void LeaveEffects()
    {
        base.LeaveEffects();

        foreach (var door in doors)
        {
            door.ToggleDoor(false);
        }
    }
}
