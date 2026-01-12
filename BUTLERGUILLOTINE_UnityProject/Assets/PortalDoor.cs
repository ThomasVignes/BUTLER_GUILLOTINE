using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoor : Interactable
{
    [SerializeField] Transform spot;
    PortalDoor linkedDoor;
    Transform target;
    PlayerSwapper playerSwapper;

    bool initialized, isLinked;

    public Transform Spot { get { return spot; } }

    public void Init(PortalDoor linkedDoor, PlayerSwapper swapper, bool isLinked)
    {
        if (initialized)
            return; 

        initialized = true;
        playerSwapper = swapper;

        this.linkedDoor = linkedDoor;

        target = linkedDoor.Spot;

        this.isLinked = isLinked;

        linkedDoor.Init(this, swapper, true);
    }

    protected override void InteractEffects(Character character)
    {
        if (!initialized)
            return;

        base.InteractEffects(character);

        StartCoroutine(C_Swap());
    }

    IEnumerator C_Swap()
    {
        GameManager.Instance.ScreenEffects.FadeTo(1, 0.1f);

        yield return new WaitForSeconds(0.1f);

        if (isLinked)
            playerSwapper.SwapBackTo(target);
        else
            playerSwapper.SwapTo(target);

        GameManager.Instance.ScreenEffects.FadeTo(0, 0.1f);
    }
}
