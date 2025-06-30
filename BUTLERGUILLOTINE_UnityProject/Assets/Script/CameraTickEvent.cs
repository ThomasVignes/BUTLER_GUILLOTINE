using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraTickEvent : MonoBehaviour
{
    [SerializeField] int ticksBeforeEvent;
    [SerializeField] UnityEvent delegates;
    [SerializeField] bool destroyOnEnd;

    bool done;

    private void Start()
    {
        GameManager.Instance.CameraTick += () => Tick();
    }

    void Tick()
    {
        if (done)
            return;

        ticksBeforeEvent--;

        if (ticksBeforeEvent <= 0)
            Event();
    }

    void Event()
    {
        GameManager.Instance.CameraTick -= () => Tick();

        delegates.Invoke();

        if (destroyOnEnd)
            gameObject.SetActive(false);
    }
}
