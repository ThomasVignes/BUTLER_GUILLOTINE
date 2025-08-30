using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class RunTutorial : Tutorial
{
    [Header("Input settings")]
    [SerializeField] private float clickdelay;

    private int clicked;
    private float clicktime;


    private void Update()
    {
        if (!active)
            return;

        if (GameManager.Instance.VNMode)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (HandleDoubleClick())
                EndTutorial();

        }
    }

    public override void Activate()
    {
        base.Activate();
    }


    private bool HandleDoubleClick()
    {
        if (Time.time - clicktime > clickdelay)
            clicked = 0;

        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicktime = Time.time;
            return true;
        }
        else if (Time.time - clicktime > clickdelay)
        {
            clicked = 0;
            clicktime = 0;
        }

        return false;
    }
}
