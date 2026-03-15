using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;
using Whumpus;

public class RoomEvent :MonoBehaviour
{
    public bool Active;
    public float TimeBeforeEvent;
    public LayerMask Layer;
    public UnityEvent OnActivate;

    bool done;

    private void Update()
    {
        if (done) return;

        if (Active)
        {
            if (TimeBeforeEvent > 0)
                TimeBeforeEvent -= Time.deltaTime;
            else
            {
                Activate();
                done = true;
            }
        }
    }


    void Activate()
    {
        OnActivate?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == WhumpusUtilities.ToLayer(Layer))
        {
            Active = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == WhumpusUtilities.ToLayer(Layer))
        {
            Active = false;
        }
    }
}
