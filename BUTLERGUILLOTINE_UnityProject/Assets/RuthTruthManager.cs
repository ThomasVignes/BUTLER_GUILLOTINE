using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RuthTruthManager : ChapterManagerGeneric
{
    [Header("Settings")]
    public bool Skip;
    public string InterSceneName;
    public DialogueCinematic startCinematic;
    [SerializeField] Animator animator;
    [SerializeField] GameObject fixedCam;
    public UnityEvent OnStart;

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
    }

    public override void IntroStep()
    {
    }

    public override void StartGame()
    {
        gameManager.ThemeManager.CreateInterScene(InterSceneName);

        if (!Skip)
        {
            Intro = false;
            gameManager.Ready = true;
            startCinematic.Play();
            return;
        }

        StartCoroutine(C_Start());
    }

    public void QuickStart()
    {
        Intro = false;
        gameManager.Ready = true;
        OnStart?.Invoke();
    }

    IEnumerator C_Start()
    {
        Intro = true;


        gameManager.ScreenEffects.SetBlackScreenAlpha(1);

        yield return new WaitForSeconds(2.3f);


        gameManager.ScreenEffects.FadeTo(0, 4f);


        yield return new WaitForSeconds(2.6f);

        Intro = false;
        gameManager.Ready = true;
    }

    public override void RestartGame()
    {
    }


    public override void EndChapter()
    {
        StartCoroutine(C_EndChapter());
    }


    IEnumerator C_EndChapter()
    {
        gameManager.ScreenEffects.FadeTo(1, 2.9f);

        yield return new WaitForSeconds(4);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void Title()
    {
    }


    public override void Death(string message)
    {
    }
}
