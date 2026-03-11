using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetPuzzle : MonoBehaviour
{
    public bool Solved;
    [SerializeField] GameObject[] targetOrder;
    public UnityEvent OnSolve, OnFailed;

    List<GameObject> hitTargets = new List<GameObject>();

    public void HitTarget(GameObject target)
    {
        if (Solved)
            return;

        if (!hitTargets.Contains(target)) hitTargets.Add(target);

        TrySolve();
    }

    public void RemoveTarget(GameObject target)
    {
        if (Solved)
            return;

        if (hitTargets.Contains(target)) hitTargets.Remove(target);
    }

    void TrySolve()
    {
        if (Solved)
            return;

        if (targetOrder.Length != hitTargets.Count) return;

        bool solved = true;

        for (int i = 0; i < targetOrder.Length; i++)
        {
            if (targetOrder[i] != hitTargets[i])
            {
                solved = false; break;
            }
        }

        if (solved)
        {
            Solved = true;
            OnSolve?.Invoke();
        }
        else
        {
            Solved = false;
            OnFailed?.Invoke();

            hitTargets.Clear();
        }
    }
}
