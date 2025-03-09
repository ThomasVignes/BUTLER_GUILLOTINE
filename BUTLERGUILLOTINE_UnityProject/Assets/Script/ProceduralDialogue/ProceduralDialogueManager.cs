using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralDialogueManager : MonoBehaviour
{
    public static ProceduralDialogueManager Instance;

    [Header("Randomizer")]
    [SerializeField] private int minLines;
    [SerializeField] private int maxLines;

    [Header("Pacing")]
    [SerializeField] private float delayBeforeDialogue;
    [SerializeField] private float delayBetweenLines;

    [Header("Data")]
    public DialogueBatch Greetings;
    public DialogueBatch Content;
    public DialogueBatch Goodbyes;

    [Header("References")]
    [SerializeField] GameObject dialogueCharacterInstance;
    [SerializeField] GameObject[] mingleCharacters;
    [SerializeField] GameObject DialogueInstance;
    [SerializeField] List<GameObject> DialogueInstances = new List<GameObject>();

    List<DialogueCharacter> dialogueCharacters = new List<DialogueCharacter>();
    List<DialogueBatchIndex> indexes = new List<DialogueBatchIndex>();

    DialogueCharacter initiator;
    DialogueCharacter interlocutor;

    GameManager gameManager;

    private void Start()
    {
        Init(GameManager.Instance);
    }

    public void Init(GameManager gameManager)
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        this.gameManager = gameManager;

        indexes.Add(new DialogueBatchIndex(Greetings.batchName));
        indexes.Add(new DialogueBatchIndex(Content.batchName));
        indexes.Add(new DialogueBatchIndex(Goodbyes.batchName));

        for (int i = 0; i < 4; i++)
        {
            CreateNewInstance();
        }

        foreach (var item in mingleCharacters)
        {
            Character c = item.GetComponentInChildren<Character>();

            GameObject go = Instantiate(dialogueCharacterInstance);
            DialogueCharacter dc = go.GetComponent<DialogueCharacter>();


            go.transform.SetParent(c.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;


            dc.Init(c);

            dialogueCharacters.Add(dc);
        }
    }

    private DialogueInstance CreateNewInstance()
    {
        GameObject objToSpawn = Instantiate(DialogueInstance);
        objToSpawn.transform.SetParent(transform);
        objToSpawn.GetComponent<DialogueInstance>().InitValues(this, minLines, maxLines, delayBeforeDialogue, delayBetweenLines);

        DialogueInstances.Add(objToSpawn);

        return objToSpawn.GetComponent<DialogueInstance>();
    }

    public void StartDialogue(DialogueCharacter initatior, DialogueCharacter interlocutor)
    {
        bool dialogueStarted = false;

        foreach (GameObject i in DialogueInstances) 
        { 
            DialogueInstance instance = i.GetComponent<DialogueInstance>();

            if (instance.Available) 
            { 
                instance.StartDialogue(initatior, interlocutor);
                dialogueStarted = true;
                break;
            }
        }

        if (!dialogueStarted) 
        {
            DialogueInstance instance = CreateNewInstance();

            instance.StartDialogue(initatior, interlocutor);
        }
    }


    public string SelectRandomLine(DialogueBatch batch)
    {
        int index = Random.Range(0, batch.lines.Length);

        while (!IndexAvailable(index, batch))
        {
            index = Random.Range(0, batch.lines.Length);
        }

        return batch.lines[index];
    }

    public bool IndexAvailable(int index, DialogueBatch batch) 
    {
        foreach (var item in indexes)
        {
            if (item.Name == batch.batchName)
            {
                if (item.Numbers.Contains(index))
                    return false;
                else
                {
                    item.UniqueBeforeRepeats++;

                    if (item.UniqueBeforeRepeats > batch.linesBeforeRepeats)
                    {
                        item.Numbers.Clear();
                        item.UniqueBeforeRepeats = 0;
                    }

                    item.Numbers.Add(index);

                    if (item.Numbers.Count >= batch.lines.Length)
                    {
                        item.Numbers.Clear();
                        item.UniqueBeforeRepeats = 0;
                    }

                    return true;
                }
            }
        }

        return false;
    }
}

public class DialogueBatchIndex
{
    public string Name;
    public List<int> Numbers = new List<int>();
    public int UniqueBeforeRepeats;

    public DialogueBatchIndex(string name)
    {
        Name = name;
    }
}