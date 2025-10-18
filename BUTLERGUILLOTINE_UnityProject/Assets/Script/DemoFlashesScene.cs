using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoFlashesScene : MonoBehaviour
{
    [SerializeField] string NextSceneName;
    [SerializeField] bool hasFlash;
    [SerializeField] float TimeBeforeFlash, FlashDuration, TimeBeforeNext;
    [SerializeField] float TimeBeforeButler, TimeBeforeGuillotine, TimebeforeDate;
    [SerializeField] Animator animator, animator2;
    [SerializeField] GameObject flash, flash2, blackScreen, direc;
    [SerializeField] GameObject butler, guillotine, date;



    private void Start()
    {
        Cursor.visible = false;
        butler.SetActive(false);
        guillotine.SetActive(false);
        date.SetActive(false);

        StartCoroutine(C_Wait());
    }

    IEnumerator C_Wait()
    {
        yield return new WaitForSeconds(TimeBeforeFlash);

        if (hasFlash)
        {
            EffectsManager.Instance.audioManager.Play("Flash");

                animator.SetTrigger("Next");


            yield return new WaitForSeconds(0.05f);
            direc.SetActive(false);
            flash.SetActive(true);

            yield return new WaitForSeconds(FlashDuration);

            flash.SetActive(false);
            direc.SetActive(true);
        }

        animator2.SetTrigger("Last");
        animator.SetTrigger("Last");

        yield return new WaitForSeconds(TimeBeforeFlash);



        if (hasFlash)
        {
            EffectsManager.Instance.audioManager.Play("Flash");

            yield return new WaitForSeconds(0.05f);
            direc.SetActive(false);
            flash2.SetActive(true);

            yield return new WaitForSeconds(FlashDuration);

            flash2.SetActive(false);
            direc.SetActive(true);
        }

        yield return new WaitForSeconds(TimeBeforeButler);

        butler.SetActive(true);

        yield return new WaitForSeconds(TimeBeforeGuillotine);

        guillotine.SetActive(true);

        yield return new WaitForSeconds(TimebeforeDate);

        date.SetActive(true);

        yield return new WaitForSeconds(TimeBeforeNext);

        NextScene();
    }

    public void NextScene()
    {
        StartCoroutine(C_NextScene());
    }

    IEnumerator C_NextScene()
    {
        EffectsManager.Instance.audioManager.Play("Flash");

        yield return new WaitForSeconds(0.05f);
        Image img = blackScreen.GetComponent<Image>();

        blackScreen.SetActive(true);

        yield return new WaitForSeconds(6);

        Cursor.visible = true;
        SceneManager.LoadScene(NextSceneName);
    }
}
