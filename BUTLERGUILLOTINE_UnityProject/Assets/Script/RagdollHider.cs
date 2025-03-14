using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagdollHider : MonoBehaviour
{
    [SerializeField] bool hideOnStart;
    [SerializeField] Vector3 hideOffset;
    [SerializeField] GameObject[] deactivateObjects;
    [SerializeField] Rigidbody rb;

    Vector3 originalPos;
    NavMeshAgent agent;

    DialogueCharacter dialogueCharacter;

    bool init;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        if (hideOnStart)
            Hide();
    }

    private void Init()
    {
        if (init)
            return;

        init = true;

        originalPos = transform.position;

        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
            agent = GetComponentInChildren<NavMeshAgent>();

        dialogueCharacter = GetComponentInChildren<DialogueCharacter>();
    }

    public void Hide()
    {
        if (!Application.isPlaying)
            return;

        if (agent == null)
            Init();

        PauseDialogueCharacter(true);

        if (agent != null)
            agent.enabled = false;

        if (rb != null)
            rb.isKinematic = true;

        transform.position = originalPos + hideOffset;

        foreach (var item in deactivateObjects)
        {
            if (item != null)
                item.SetActive(false);
        }
    }

    [ContextMenu("Show")]
    public void Show()
    {
        if (!Application.isPlaying)
            return;

        if (agent == null)
            Init();

        PauseDialogueCharacter(false);

        transform.position = originalPos;

        foreach (var item in deactivateObjects)
        {
            if (item != null)
                item.SetActive(true);
        }

        if (agent != null)
            agent.enabled = true;

        if (rb != null)
            rb.isKinematic = false;
    }


    private void PauseDialogueCharacter(bool pause)
    {
        if (dialogueCharacter == null)
            dialogueCharacter = GetComponentInChildren<DialogueCharacter>();

        if (dialogueCharacter != null)
            dialogueCharacter.Paused = pause;
    }
}
