using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSwapper : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool preInstantiatePlayer;
    [SerializeField] float swapDelay;

    [Header("Player")]
    [SerializeField] string targetPlayer;
    [SerializeField] string original;
    [SerializeField] Transform targetPos;
    [SerializeField] Transform originalTargetPos;
    [SerializeField] Transform preinstantiateSpot;
    [SerializeField] bool hideOriginal;

    [Header("Delegate")]
    public UnityEvent onSwap;
    public UnityEvent onBack;
    

    private void Start()
    {
        if (preInstantiatePlayer)
        {
            if (preinstantiateSpot == null)
                GameManager.Instance.PreInstantiatePlayer(targetPlayer, targetPos);
            else
                GameManager.Instance.PreInstantiatePlayer(targetPlayer, preinstantiateSpot);
        }
    }

    [ContextMenu("Manual Swap")]
    public void Swap()
    {
        StartCoroutine(C_SwapDelay());
    }

    IEnumerator C_SwapDelay()
    {
        yield return new WaitForSeconds(swapDelay);

        GameManager.Instance.SwapPlayer(targetPlayer, targetPos, hideOriginal);

        onSwap?.Invoke();
    }

    public void SwapBack()
    {
        StartCoroutine(C_SwapBackDelay());
    }

    IEnumerator C_SwapBackDelay()
    {
        yield return new WaitForSeconds(swapDelay);

        GameManager.Instance.SwapPlayer(original, originalTargetPos, hideOriginal);

        onBack?.Invoke();
    }
}
