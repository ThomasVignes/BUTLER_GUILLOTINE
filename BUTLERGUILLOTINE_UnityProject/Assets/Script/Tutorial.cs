using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    public string tutorialMessage;
    public TextMeshProUGUI tutorialText;

    public bool active;

    public virtual void Activate()
    {
        if (active)
            return;

        active = true;

        tutorialText.text = tutorialMessage;

        tutorialText.DOFade(1, 0.5f);
    }


    public virtual void EndTutorial()
    {
        active = false;

        tutorialText.DOKill();
        tutorialText.DOFade(0, 0.2f);
    }

}
