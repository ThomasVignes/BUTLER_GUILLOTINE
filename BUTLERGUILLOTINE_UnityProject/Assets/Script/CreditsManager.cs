using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] float scrollSpeed, creditsDuration, stillDuration;
    [SerializeField] RectTransform scroller;
    public UnityEvent OnStart;
    /*
    [SerializeField] Button skipButton;
    [SerializeField] Image skipButtonImage;
    [SerializeField] TextMeshProUGUI skipText;
    */

    [Header("Effects")]
    [SerializeField] Image blackScreen;

    [Header("Debug")]
    [SerializeField] GameObject toMenu;

    float creditsTimer, stillTimer;

    bool active;

    Vector3 originalPos;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        originalPos = scroller.localPosition;

        ResetCredits();

        StartCoroutine(C_Init());
    }

    IEnumerator C_Init()
    {
        blackScreen.DOFade(1, 0.0001f);

        yield return new WaitForSeconds(2.3f);

        blackScreen.DOFade(0, 0.4f);
        OnStart?.Invoke();

        yield return new WaitForSeconds(2.6f);

        Roll();
    }

    public void Roll()
    {
        active = true;

        creditsTimer = Time.time + creditsDuration + 1;
        stillTimer = Time.time + stillDuration + 1;


        /*
        skipButton.gameObject.SetActive(true);
        skipButtonImage.DOFade(1, 1);
        skipText.DOFade(1, 1);
        */
    }

    public void ResetCredits()
    {
        active = false;

        /*
        skipButton.gameObject.SetActive(false);
        skipButtonImage.DOFade(0, 0.0001f);
        skipText.DOFade(0, 0.0001f);
        */

        scroller.localPosition = originalPos;
    }

    private void Update()
    {
        if (!active)
            return;

        if (creditsTimer < Time.time)
        {
            EnableOptions();
        }

        if (stillTimer < Time.time)
            scroller.position += Vector3.up * scrollSpeed * Time.deltaTime;
    }

    public void EnableOptions()
    {
        active = false;

        toMenu.SetActive(true);
    }

    public void ToMenu()
    {
        StartCoroutine(C_ToMenu());
    }

    IEnumerator C_ToMenu()
    {
        toMenu.SetActive(false);

        blackScreen.DOFade(1, 2.9f);

        yield return new WaitForSeconds(4);

        SceneManager.LoadScene(0);
    }
}
