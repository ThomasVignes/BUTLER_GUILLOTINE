using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Door : Interactable
{
    [Header("Values")]
    public bool StartOpen;
    [SerializeField] private string lockedMessage;
    [SerializeField] bool reversedOpen;
    public bool CanOpen;

    [Header("Unlocking")]
    public bool InstaUnlock = true;
    [SerializeField] string keyID;

    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Events")]
    public UnityEvent OnOpen;
    public UnityEvent OnUnlock;




    [Header("Experimental")]
    public bool UpdateOnEnable;

    private bool isOpen;

    bool unlocked;

    public string LockedMessage { get { return lockedMessage; } set {  lockedMessage = value; } }

    private void Start()
    {
        ToggleDoor(StartOpen);
    }

    protected override void InteractEffects(Character character)
    {
        if (keyID != "" && !unlocked)
        {
            if (keyID == GameManager.Instance.EquippedItemID)
            {
                if (InstaUnlock)
                    Unlock();
                else
                {
                    Unlock();
                    return;
                }
            }
        }

        if (CanOpen)
        {
            if (!isOpen)
                ToggleDoor(true);
        }
        else
        {
            if (lockedMessage != "")
                GameManager.Instance.WriteComment(lockedMessage);
        }
    }

    private void OnEnable()
    {
        if (!UpdateOnEnable)
            return;

        ToggleDoor(isOpen);
    }

    public void ToggleDoor(bool open)
    {
        animator.SetBool("Open", open);
        animator.SetBool("Reversed", reversedOpen);

        isOpen = open;

        GetComponent<BoxCollider>().enabled = !open;
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();

        if (obstacle != null)
            obstacle.enabled = !open;

        if (open)
            OnOpen?.Invoke(); 
    }

    public void ToggleDoorNoEvent(bool open)
    {
        animator.SetBool("Open", open);

        isOpen = open;

        GetComponent<BoxCollider>().enabled = !open;
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();

        if (obstacle != null)
            obstacle.enabled = !open;
    }

    public void Unlock()
    {
        unlocked = true;

        CanOpen = true;

        OnUnlock?.Invoke();
    }

    public void ChangeLockMessage(string message)
    {
        lockedMessage = message;
    }
}
