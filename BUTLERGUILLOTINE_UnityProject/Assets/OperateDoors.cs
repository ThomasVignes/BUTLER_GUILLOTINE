using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OperateDoors : MonoBehaviour
{
    [SerializeField] float delayPerDoor;
    [SerializeField] GameObject[] Doors;
    [SerializeField] bool open;

    public UnityEvent OnEnd;

    Coroutine coroutine;

    public void OpenAllDoors()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(C_OpenAllDoors());
    }

    IEnumerator C_OpenAllDoors()
    {
        foreach (var item in Doors)
        {
            Door door = item.GetComponent<Door>();

            if (door == null)
                door = item.GetComponentInChildren<Door>(); 

            if (door != null)
                door.ToggleDoor(open);

            yield return new WaitForSeconds(delayPerDoor);
        }

        OnEnd?.Invoke();
    }
}
