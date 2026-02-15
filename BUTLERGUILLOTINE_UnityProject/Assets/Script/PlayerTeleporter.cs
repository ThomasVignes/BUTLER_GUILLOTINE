using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTeleporter : MonoBehaviour
{
    [SerializeField] bool blackScreen = true;
    [SerializeField] float teleportDelay;
    [SerializeField] Transform up, down;
    [SerializeField] Transform upTargetMove, downTargetMove;
    public UnityEvent OnTeleportUp, OnTeleportDown;

    public void TeleportUp()
    {
        GameManager.Instance.TeleportPlayer(up, teleportDelay, blackScreen);

        StartCoroutine(C_TeleportUpEffects());
    }

    public void TeleportDown()
    {
        GameManager.Instance.TeleportPlayer(down, teleportDelay, blackScreen);

        StartCoroutine(C_TeleportDownEffects());
    }

    IEnumerator C_TeleportUpEffects()
    {
        yield return new WaitForSeconds(teleportDelay/2 + 0.1f);

        OnTeleportUp?.Invoke();
    }

    IEnumerator C_TeleportDownEffects()
    {
        yield return new WaitForSeconds(teleportDelay/2 + 0.1f);

        OnTeleportDown?.Invoke();
    }
}
