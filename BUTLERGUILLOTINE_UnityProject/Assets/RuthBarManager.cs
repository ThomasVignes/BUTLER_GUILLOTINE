using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RuthBarManager : ChapterManagerGeneric
{
    [Header("Settings")]
    public bool Skip;
    public DialogueCinematic startCinematic;
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
        gameManager.ScreenEffects.FadeTo(1, 0.3f);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void Title()
    {
    }


    public override void Death(string message)
    {
    }
}
