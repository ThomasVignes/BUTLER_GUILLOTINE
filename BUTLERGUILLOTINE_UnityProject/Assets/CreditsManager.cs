using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] float scrollSpeed, creditsDuration, stillDuration;
    [SerializeField] RectTransform scroller;
    [SerializeField] Button skipButton;
    [SerializeField] Image skipButtonImage;
    [SerializeField] TextMeshProUGUI skipText;

    float creditsTimer, stillTimer;

    bool active;

    Vector3 originalPos;

    MainMenuMaster master;

    public void Init(MainMenuMaster master)
    {
        this.master = master;

        originalPos = scroller.localPosition;

        ResetCredits();
    }

    public void Roll()
    {
        active = true;

        creditsTimer = Time.time + creditsDuration + 1;
        stillTimer = Time.time + stillDuration + 1;

        

        skipButton.gameObject.SetActive(true);
        skipButtonImage.DOFade(1, 1);
        skipText.DOFade(1, 1);
    }

    public void ResetCredits()
    {
        active = false;

        skipButton.gameObject.SetActive(false);
        skipButtonImage.DOFade(0, 0.0001f);
        skipText.DOFade(0, 0.0001f);

        scroller.localPosition = originalPos;
    }

    private void Update()
    {
        if (!active)
            return;

        if (creditsTimer < Time.time)
        {
            master.EndCredits();
        }

        if (stillTimer < Time.time)
            scroller.position += Vector3.up * scrollSpeed * Time.deltaTime;
    }
}
