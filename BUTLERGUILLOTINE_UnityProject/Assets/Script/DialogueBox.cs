using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DialogueBox : Interactable
{
    [Header("DialogueBox Settings (Local)")]
    public UnityEvent OnDialogueEnd;
    [SerializeField] private bool once;
    [SerializeField] private float delayBeforeDelegates;
    [SerializeField] private float delayBeforeSwitch;
    [SerializeField] private string puppet;
    [SerializeField] private int dialogueReference;
    [SerializeField] bool noCam, hidePlayer;

    bool pressed;

    protected override void InteractEffects(Character character)
    {
        if (pressed)
            return;

        StartCoroutine(C_SwitchDialogue());
    }

    public void End()
    {
        if (hidePlayer)
            GameManager.Instance.HidePlayer(false);

        if (!gameObject.activeSelf)
            return;

        StartCoroutine(C_End());
    }

    IEnumerator C_End()
    {
        yield return new WaitForSeconds(delayBeforeDelegates);

        pressed = false;

        OnDialogueEnd?.Invoke();

        if (once)
            gameObject.SetActive(false);
    }

    IEnumerator C_SwitchDialogue()
    {
        pressed = true;
        GameManager.Instance.PausePlayerPath();

        yield return new WaitForSeconds(delayBeforeSwitch);

        GameManager.Instance.SetVNMode(true, noCam);
        GameManager.Instance.PausePlayerPath();
        if (hidePlayer)
            GameManager.Instance.HidePlayer(true);
        GameManager.Instance.DialogueManager.StartDialogue(puppet, dialogueReference, this);

    }

    public void ChangeReference(int index)
    {
        dialogueReference = index;
    }
}
