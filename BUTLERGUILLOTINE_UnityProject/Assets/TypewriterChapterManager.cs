using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TypewriterChapterManager: ChapterManagerGeneric
{

    public override void StartGame()
    {
        StartCoroutine(C_Start());
    }

    public override void IntroStep()
    {

    }

    public override void RestartGame()
    {
        StartCoroutine(C_Restart());
    }

    public override void Death(string message)
    {

    }

    public override void EndChapter()
    {

    }


    IEnumerator C_EndChapter()
    {
        gameManager.ScreenEffects.FadeTo(1, 0f);

        yield return new WaitForSeconds(6f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator C_Start()
    {
        gameManager.ScreenEffects.FadeTo(1, 0.01f);

        yield return new WaitForSeconds(1f);

        RestartGame();
    }

    IEnumerator C_Restart()
    {
        gameManager.ScreenEffects.FadeTo(0, 4f);

        yield return new WaitForSeconds(2.6f);

        gameManager.Ready = true;
    }
}
