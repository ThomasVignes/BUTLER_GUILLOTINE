using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiTrigger : MonoBehaviour
{
    public int PriorityTriggers;
    public int NumberBeforeTrigger;
    public UnityEvent Delegates;

    bool done;

    List<GameObject> priorities = new List<GameObject>();

    public void PriorityTrigger(GameObject go)
    {
        if (done)
            return;

        if (priorities.Contains(go))
            return;


        priorities.Add(go);

        PriorityTriggers--;


        if (NumberBeforeTrigger <= 0 && PriorityTriggers <= 0)
        {
            done = true;
            Delegates.Invoke();
        }
    }

    public void Trigger()
    {
        if (done)
            return;

        NumberBeforeTrigger--;

        if (NumberBeforeTrigger <= 0 && PriorityTriggers <= 0)
        {
            done = true;
            Delegates.Invoke();
        }
    }

    public void Disable()
    {
        done = true;
    }
}
