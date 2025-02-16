using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwapper : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool preInstantiatePlayer;
    [SerializeField] float swapDelay;

    [Header("Player")]
    [SerializeField] string targetPlayer;
    [SerializeField] Transform targetPos;
    [SerializeField] bool hideOriginal;

    private void Start()
    {
        if (preInstantiatePlayer)
            GameManager.Instance.PreInstantiatePlayer(targetPlayer, targetPos);
    }

    public void Swap()
    {
        StartCoroutine(C_SwapDelay());
    }

    IEnumerator C_SwapDelay()
    {
        yield return new WaitForSeconds(swapDelay);

        GameManager.Instance.SwapPlayer(targetPlayer, targetPos, hideOriginal);
    }
}
