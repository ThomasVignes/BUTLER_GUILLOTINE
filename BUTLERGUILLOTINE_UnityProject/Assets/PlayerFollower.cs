using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    PlayerController player;

    public void SetTarget(PlayerController player)
    {
        this.player = player;
    }

    void Update()
    {
        if (player == null)
            return;

        transform.position = player.transform.position;
    }
}
