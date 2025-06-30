using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Dialogue))]
public class AddDialogue : MonoBehaviour
{
    [SerializeField] DialogueBox box;

    private void Start()
    {
        DialogueManager manager = GameManager.Instance.DialogueManager;
        Dialogue dialogue = GetComponent<Dialogue>();

        if (manager != null && dialogue != null)
        {
            manager.AddDialogue(dialogue);

            if (box != null)
                box.ChangeReference(GameManager.Instance.DialogueManager.Dialogues.IndexOf(dialogue));
        }
    }
}
