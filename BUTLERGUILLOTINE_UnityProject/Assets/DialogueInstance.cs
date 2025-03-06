using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInstance : MonoBehaviour
{
    private int minLines;
    [SerializeField] private int maxLines;

    private float delayBeforeDialogue;
    private float delayBetweenLines;

    private DialogueCharacter initiator;
    private DialogueCharacter interlocutor;

    ProceduralDialogueManager manager;

    bool speaking;

    public bool Available { get { return !speaking; } }

    public void InitValues(ProceduralDialogueManager manager, int minLines, int maxLines, float delayBeforeDialogue, float delayBetweenLines)
    {
        this.manager = manager;
        this.minLines = minLines;
        this.maxLines = maxLines;
        this.delayBeforeDialogue = delayBeforeDialogue;
        this.delayBetweenLines = delayBetweenLines;
    }

    public void StartDialogue(DialogueCharacter initatior, DialogueCharacter interlocutor)
    {
        speaking = true;

        this.initiator = initatior;
        this.interlocutor = interlocutor;

        initiator.StartDialogue();
        interlocutor.StartDialogue();

        StartCoroutine(StartDialogue());
    }
    public void EndDialogue()
    {
        speaking = false;

        initiator.EndDialogue();
        interlocutor.EndDialogue();
    }

    IEnumerator StartDialogue()
    {
        int linesNumber = Random.Range(minLines, maxLines);


        yield return new WaitForSeconds(delayBeforeDialogue);


        initiator.Speak(manager.SelectRandomLine(manager.Greetings));

        yield return new WaitForSeconds(delayBetweenLines);

        interlocutor.Speak(manager.SelectRandomLine(manager.Greetings));

        yield return new WaitForSeconds(delayBetweenLines);


        for (int i = 0; i < linesNumber; i++)
        {
            initiator.Speak(manager.SelectRandomLine(manager.Content));

            yield return new WaitForSeconds(delayBetweenLines);

            interlocutor.Speak(manager.SelectRandomLine(manager.Content));

            yield return new WaitForSeconds(delayBetweenLines);
        }


        yield return new WaitForSeconds(delayBetweenLines);

        initiator.Speak(manager.SelectRandomLine(manager.Goodbyes));

        yield return new WaitForSeconds(delayBetweenLines);

        interlocutor.Speak(manager.SelectRandomLine(manager.Goodbyes));

        yield return new WaitForSeconds(delayBetweenLines);


        EndDialogue();
    }
}
