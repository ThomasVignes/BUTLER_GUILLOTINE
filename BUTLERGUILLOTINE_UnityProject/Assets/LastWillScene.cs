using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastWillScene : MonoBehaviour
{
    [SerializeField] string NextSceneName;
    [SerializeField] bool hasFlash;
    [SerializeField] float TimeBeforeFlash, FlashDuration, TimeBeforeNext;
    [SerializeField] Animator animator;
    [SerializeField] GameObject flash, blackScreen;

    private void Start()
    {
        Cursor.visible = false;
        StartCoroutine(C_Wait());
    }

    IEnumerator C_Wait()
    {
        yield return new WaitForSeconds(TimeBeforeFlash);

        if (hasFlash)
        {
            animator.SetTrigger("FastFlash");

            yield return new WaitForSeconds(0.05f);
            flash.SetActive(true);

            yield return new WaitForSeconds(FlashDuration);

            flash.SetActive(false);
        }

        yield return new WaitForSeconds(TimeBeforeNext);

        NextScene();
    }

    public void NextScene()
    {
        StartCoroutine(C_NextScene());
    }

    IEnumerator C_NextScene()
    {
        animator.SetTrigger("FastFlash");

        yield return new WaitForSeconds(0.05f);
        blackScreen.SetActive(true);

        yield return new WaitForSeconds(FlashDuration);

        Cursor.visible = true;
        SceneManager.LoadScene(NextSceneName);
    }
}
