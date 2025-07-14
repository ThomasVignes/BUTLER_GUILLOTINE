using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class IntroManager : ChapterManagerGeneric
{
    [Header("Settings")]
    public bool Skip;
    [SerializeField] private string startCinematic;
    [SerializeField] private float endTime;
    [SerializeField] Animator animator;

    [SerializeField] GameObject fixedCam;



    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
    }

    public override void IntroStep()
    {
    }

    public override void StartGame()
    {
        gameManager.ThemeManager.CreateInterScene("LaSonnambula");

        if (Skip)
        {
            gameManager.Ready = true;

            gameManager.StopOverride();

            return;
        }

        StartCoroutine(C_Start());
    }

    IEnumerator C_Start()
    {
        Intro = true;

        
        gameManager.ScreenEffects.FadeTo(1, 0.01f);

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
        EffectsManager.Instance.audioManager.Play("Flash");
        animator.SetTrigger("Blast");

        yield return new WaitForSeconds(0.05f);

        SceneManager.LoadScene("INTRO_1");
    }


    public void Title()
    {
    }


    public override void Death(string message)
    {
    }
}
