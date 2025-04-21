using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenEffects : MonoBehaviour
{
    [SerializeField] private Image blackScreen, oumuamua;

    public void StartFade()
    {
        blackScreen.DOKill();
        oumuamua.DOFade(0, 0.01f);
        FadeTo(1, 0.01f);
        FadeTo(0, 3.3f);
    }

    public void SetBlackScreenAlpha(float amount)
    {
        Color color = blackScreen.color;
        color.a = amount;

        blackScreen.color = color;
    }

    public void FadeTo(float amount, float duration)
    {
        blackScreen.DOKill();
        blackScreen.DOFade(amount, duration);
    }

    public void OumuamuaFade()
    {
        oumuamua.DOFade(0.8f, 7);
    }

    public void FadeInOut(float delayWait)
    {
        blackScreen.DOKill();
        StartCoroutine(C_FadeInOut(delayWait));
    }

    IEnumerator C_FadeInOut(float delayWait)
    {
        blackScreen.DOFade(1, 1f);

        yield return new WaitForSeconds(delayWait + 1f);

        blackScreen.DOFade(0, 1f);
    }
}
