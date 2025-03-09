using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueCharacter : Interactable
{
    public bool Paused;

    [Header("AI")]
    [SerializeField] bool freeRoam;
    [SerializeField] float roamRadius;
    [SerializeField] float minIdle, maxIdle;
    [SerializeField] private int minCD, maxCD;

    [Header("References")]
    [SerializeField] Character character;
    [SerializeField] TextMeshProUGUI speakText;
    [SerializeField] GameObject canvas;

    bool ready, mingling, available;
    float waitTimer;

    bool justMingled;
    int mingleCD;

    GameObject last;


    public bool Available { get { return available; } }
    public bool Mingling { get { return mingling; } }

    DialogueCharacter initiator;

    DialogueCharacter lastMingler;

    PointOfInterest poi;

    public void Init(Character character)
    {
        if (this.character == null)
            this.character = character;

        ready = true;

        if (freeRoam)
        {
            SetWalkTimer();
        }

        available = true;
    }

    private void Update()
    {
        if (!ready)
            return;

        if (Paused)
            return;

        if (freeRoam)
        {
            if (poi != null && !poi.Available && character.Moving)
            {
                poi = null;
                waitTimer = 0;
                TryWalk();
            }

            if (waitTimer > 0 && !character.Moving)
            {
                waitTimer -= Time.deltaTime;
            }

            if (mingling || !available)
                return;

            if (waitTimer <= 0)
            {
                TryWalk();
            }
        }
    }


    public void TryWalk()
    {
        SetWalkTimer();

        Vector3 targPos = transform.position;

        Collider[] cols = Physics.OverlapSphere(transform.position, roamRadius);

        Reshuffle(cols);

        foreach (var item in cols)
        {
            if (item.gameObject != gameObject && item.gameObject != last)
            {
                DialogueCharacter character = item.GetComponent<DialogueCharacter>();

                if (character != null && character.Available && !justMingled && character != lastMingler)
                {
                    targPos = item.transform.position;
                    character.Call(this);
                    available = false;

                    this.character.SetDestination(character.GetTargetPosition(), character);

                    last = item.gameObject;

                    lastMingler = character;

                    ResetMingle();
                    return;
                }

                poi = item.GetComponent<PointOfInterest>();

                if (poi != null && poi.Available)
                {
                    targPos = item.transform.position;
                    this.character.SetDestination(targPos, poi);
                    last = item.gameObject;

                    MingleCD();
                    return;
                }
            }
        }

        
    }

    [ContextMenu("TrySpeak")]
    public void SpeakTo()
    {
        if (initiator != null)
        {
            character.Pause();
            ProceduralDialogueManager.Instance.StartDialogue(initiator, this);
        }
    }

    public void Call(DialogueCharacter initiator)
    { 
        this.initiator = initiator;
        available = false;

        last = initiator.gameObject;

        lastMingler = initiator;

        character.Pause();
    }

    public void Speak(string text)
    {
        speakText.text = text;
    }

    public void StartDialogue()
    {
        speakText.text = "";
        canvas.SetActive(true);

        mingling = true;
    }

    public void EndDialogue()
    {
        speakText.text = "";
        canvas.SetActive(false);

        mingling = false;
        available = true;

        SetWalkTimer();
    }

    public void SetWalkTimer()
    {
        float timer = Random.Range(minIdle, maxIdle);

        waitTimer = timer;
    }

    public void ResetMingle()
    {
        justMingled = true;
        mingleCD = Random.Range(minCD, maxCD);
    }

    public void MingleCD()
    {
        if (!justMingled)
            return;

        mingleCD--;

        if (mingleCD <= 0)
        {
            justMingled = false;
        }
    }

    protected override void InteractEffects(Character character)
    {
        if (mingling)
            return;

        SpeakTo();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }

    void Reshuffle(Collider[] texts)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < texts.Length; t++)
        {
            Collider tmp = texts[t];
            int r = Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }
    }
}
