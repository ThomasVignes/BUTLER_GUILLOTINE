using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RuthRitualManger : ChapterManagerGeneric
{
    public bool Skip;
    [SerializeField] private string startCinematic;
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
        OnStart?.Invoke();

        if (Skip)
        {
            gameManager.Ready = true;
            return;
        }

        StartCoroutine(C_Start());
    }

    IEnumerator C_Start()
    {
        gameManager.ScreenEffects.FadeTo(1, 0.01f);
        PersistentData.Instance.MusicVolume = 0;

        yield return new WaitForSeconds(2.3f);

        PersistentData.Instance.MusicVolume = 1;

        gameManager.CinematicManager.PlayCinematic(startCinematic);
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

    public override void Death(string message)
    {

    }
}
