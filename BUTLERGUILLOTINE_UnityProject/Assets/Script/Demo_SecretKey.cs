using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Demo_SecretKey : MonoBehaviour
{
    [SerializeField] string hasKeyMessage;

    [Header("Standalone comment")]
    [SerializeField] private TextMeshProUGUI observationDialogue;
    [SerializeField] GameObject Specific, Blocker;

    [Header("References")]
    [SerializeField] GameObject[] done;
    [SerializeField] GameObject[] toHide;
    [SerializeField] GameObject[] toShow;
    [SerializeField] RagdollHider alive, dead;

    bool writing, skip;
    private bool specific, endSpecific;

    float delayBetweenLetters = 0.02f;
    float strongPuncWait = 0.4f;
    float lightPuncWait = 0.07f;


    private void Start()
    {
        if (PersistentData.Instance.HasKey)
        {
            Done();
            return;
        }

        bool showSecret = PersistentData.Instance.FinishedOnce;

        if (showSecret)
            ShowSecret();
        else
            HideSecret();
    }

    [ContextMenu("Comment")]
    public void Comment()
    {
        WriteSpecific("Speak speak speak speak speak speak speak speak speak ");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (writing)
                skip = true;

            if (endSpecific)
            {
                EndSpecific();
            }
        }
    }

    public void GotKey()
    {
        WriteSpecific(hasKeyMessage);
        PersistentData.Instance.HasKey = true;
    }

    void Done()
    {
        foreach (var item in done)
        {
            item.SetActive(true);
        }

        alive.Hide();
        dead.Hide();

        foreach (var item in toHide)
            item.SetActive(false);

        foreach (var item in toShow)
            item.SetActive(false);
    }

    void ShowSecret()
    {
        alive.Hide();
        dead.Show();

        foreach (var item in toHide)
            item.SetActive(false);

        foreach (var item in toShow)
            item.SetActive(true);
    }

    void HideSecret()
    {
        alive.Show();
        dead.Hide();

        foreach (var item in toHide)
            item.SetActive(true);

        foreach (var item in toShow)
            item.SetActive(false);
    }

    public void WriteSpecific(string text)
    {
        StartCoroutine(C_Specific(text));
    }

    IEnumerator C_Specific(string text)
    {
        specific = true;

        Specific.SetActive(true);
        Blocker.SetActive(true);

        observationDialogue.text = "";

        writing = true;

        var charCount = 0;

        foreach (char c in text)
        {
            observationDialogue.text += c;

            EffectsManager.Instance.audioManager.Play("SmallClick");

            if (skip)
            {
                break;
            }

            charCount++;

            string strongPunctuations = ".?!";
            string lightPunctuations = ",:";

            var last = charCount - 1;

            if (last < 0)
                last = 0;

            if (strongPunctuations.Contains(c) && charCount < text.Length - 1 && text[last] != c)
                yield return new WaitForSeconds(strongPuncWait);
            else if (lightPunctuations.Contains(c))
                yield return new WaitForSeconds(lightPuncWait);
            else
                yield return new WaitForSeconds(delayBetweenLetters);

            //yield return new WaitForSeconds(delayBetweenLetters);
        }

        writing = false;

        if (skip)
        {
            observationDialogue.text = text;
            skip = false;
        }

        endSpecific = true;
    }

    public void EndSpecific()
    {
        specific = false;
        endSpecific = false;

        StartCoroutine(C_EndSpecific());
    }

    IEnumerator C_EndSpecific()
    {
        Specific.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        Blocker.SetActive(false);
    }

}
