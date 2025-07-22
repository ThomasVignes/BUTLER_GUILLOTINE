using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

public class DialogueCinematic : MonoBehaviour
{
    [Header("Content (duration does nothing)")]
    public DialogueCinematicLine[] lines;

    [Header("Settings")]
    [SerializeField] string Ambience;
    [SerializeField] float delayBetweenLetters;
    [SerializeField] bool autoEnter;
    [SerializeField] bool dontRemoveLines;
    public UnityEvent OnEachLineEnd;

    [Header("UI")]
    [SerializeField] string writeWait;
    [SerializeField] float flickerDelay;

    [Header("BlackScreens")]
    public float StartBlackScreenDuration;
    public float EndBlackScreenDuration;
    [SerializeField] bool instaFade;
    public float StartBlackScreenFade, EndBlackScreenFade = 1;
    public bool noOpeningBlackScreen;
    [SerializeField] bool noEndingBlackScreen;

    public UnityEvent OnStart;
    public UnityEvent OnEndBeforeBlackScreen;
    public UnityEvent OnEnd;

    [Header("References")]
    [SerializeField] GameObject Interface;
    [SerializeField] GameObject Camera;
    [SerializeField] TextMeshProUGUI textUI;
    public CinematicPuppet[] CinematicPuppets;


    [Header("Experimental")]
    [SerializeField] bool lastCinematic;
    [SerializeField] bool dontDeleteLines;
    bool playingDialogue;


    bool writing, skip;
    bool canSkip;

    int currentLineIndex;

    float strongPuncWait, lightPuncWait;

    private void Start()
    {
        strongPuncWait = GameManager.Instance.StrongPunctuationWait;
        lightPuncWait = GameManager.Instance.LightPunctuationWait;

        foreach (var item in CinematicPuppets)
        {
            item.Animator = item.Puppet.GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (playingDialogue)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (writing)
                {
                    if (!skip)
                        skip = true;
                }
                else
                    Next();
            }
        }
    }

    public void CanSkip()
    {
        canSkip = true;
    }

    public void Next()
    {
        if (!playingDialogue)
            return;

        currentLineIndex++;

        if (currentLineIndex < lines.Length)
        {
            StartCoroutine(C_WriteDialogue());
            EffectsManager.Instance.audioManager.Play("SmallValidate");
        }
        else
        {
            EffectsManager.Instance.audioManager.Play("SmallValidate");
            DialogueFinished();
        }
    }

    public void DialogueFinished()
    {
        playingDialogue = false;

        var gameManager = GameManager.Instance;

        Camera.SetActive(false);
        Interface.SetActive(false);

        if (!noEndingBlackScreen)
        {
            StartCoroutine(C_EndBlackScreen(gameManager));
        }
        else
        {
            Camera.SetActive(false);
            Interface.SetActive(false);

            TryEndDialogue(gameManager);
        }
    }

    IEnumerator C_EndBlackScreen(GameManager gameManager)
    {
        if (!noEndingBlackScreen)
        {
            if (instaFade)
                gameManager.ScreenEffects.FadeTo(1, 0.0001f);
            else
                gameManager.ScreenEffects.FadeTo(1, 0.2f);
        }

        yield return new WaitForSeconds(0.2f);

        Camera.SetActive(false);
        Interface.SetActive(false);

        OnEndBeforeBlackScreen?.Invoke();

        if (!noEndingBlackScreen)
        {
            yield return new WaitForSeconds(EndBlackScreenDuration);
            gameManager.ScreenEffects.FadeTo(0, EndBlackScreenFade);
            yield return new WaitForSeconds(EndBlackScreenFade - 0.2f);
        }

        TryEndDialogue(gameManager);
    }

    public void TryEndDialogue(GameManager gameManager)
    {
        if (gameManager != null)
            gameManager.SetVNMode(false, false);

        OnEnd?.Invoke();

        writing = false;

        playingDialogue = false;
    }

    [ContextMenu("Play")]
    public void Play()
    {
        var gameManager = GameManager.Instance;

        if (gameManager != null)
            gameManager.SetVNMode(true, false);

        Interface.SetActive(true);
        Camera.SetActive(true);

        currentLineIndex = 0;

        OnStart?.Invoke();

        textUI.text = "";

        StartCoroutine(C_StartDialogueCinematic(gameManager));
    }

    IEnumerator C_StartDialogueCinematic(GameManager gameManager)
    {
        if (Ambience != "")
            gameManager.OverrideAmbiance(Ambience);

        if (!noOpeningBlackScreen)
        {
            if (instaFade)
                gameManager.ScreenEffects.FadeTo(1, 0.001f);
            else
                gameManager.ScreenEffects.FadeTo(1, StartBlackScreenFade);

            yield return new WaitForSeconds(0.2f);

            yield return new WaitForSeconds(StartBlackScreenDuration);

            gameManager.ScreenEffects.FadeTo(0, StartBlackScreenFade);

            yield return new WaitForSeconds(StartBlackScreenFade - 0.2f);
        }


        playingDialogue = true;

        StartCoroutine(C_WriteDialogue());
    }

    IEnumerator C_WriteDialogue()
    {
        writing = true;

        if (!dontRemoveLines)
            textUI.text = "";
        else if (autoEnter)
        {
            textUI.text += "\n\n";
        }

        var line = lines[currentLineIndex];

        if (line.DeletesPreviousText)
        {
            textUI.text = "";
        }

        var before = textUI.text;

        //Play animations
        foreach (var item in line.PuppetActions)
        {
            PlayPuppetAction(item.PuppetName, item.Action);
        }

        int charCount = 0;

        foreach (char c in line.Text)
        {
            if (skip)
            {
                break;
            }

            textUI.text += c;
            charCount++;

            EffectsManager.Instance.audioManager.Play("SmallClick");

            string strongPunctuations = ".?!";
            string lightPunctuations = ",:";

            if (strongPunctuations.Contains(c) && charCount < line.Text.Length - 1)
                yield return new WaitForSeconds(strongPuncWait);
            else if (lightPunctuations.Contains(c))
                yield return new WaitForSeconds(lightPuncWait);
            else
                yield return new WaitForSeconds(delayBetweenLetters);
        }

        if (skip)
        {
            textUI.text = before + line.Text;
            skip = false;
        }

        OnEachLineEnd?.Invoke();

        writing = false;
    }

    public void PlayPuppetAction(string puppet, string action)
    {
        CinematicPuppet p = Array.Find(CinematicPuppets, p => p.Name == puppet);

        if (p == null)
            return;

        p.Animator.SetTrigger(action);
    }
}

[System.Serializable]
public class DialogueCinematicLine
{
    public string Text;
    public bool DeletesPreviousText;
    public PuppetAction[] PuppetActions;
    public CameraEffect cameraEffect;
    public string newAmbience;
    public string[] soundEffects;
    public UnityEvent Delegates;
}
